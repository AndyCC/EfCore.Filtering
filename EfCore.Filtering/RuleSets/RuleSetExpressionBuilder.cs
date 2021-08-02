using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCore.Filtering.RuleSets
{
    /// <summary>
    /// Defines how to interpret a ruleset and build an expression
    /// </summary>
    public class RuleSetExpressionBuilder : IRuleSetExpressionBuilder
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ruleExpressionBuilders">RuleExpressionBuilders to build expressions from rules</param>
        public RuleSetExpressionBuilder(params IRuleExpressionBuilder[] ruleExpressionBuilders)
        {
            if (ruleExpressionBuilders == null)
                throw new ArgumentNullException(nameof(ruleExpressionBuilders));

            if (ruleExpressionBuilders.Length == 0)
                throw new ArgumentException("Must have at least 1", nameof(ruleExpressionBuilders));

            _ruleExpressionBuilders = ruleExpressionBuilders;

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;
            _anyMethod = typeof(Enumerable).GetGenericMethod("Any", bindingFlags, new ParameterTypeInfo(typeof(IEnumerable<>)), new ParameterTypeInfo(typeof(Func<,>)));
        }

        public RuleSetExpressionBuilder(IEnumerable<IRuleExpressionBuilder> ruleExpressionBuilders)
            : this(ruleExpressionBuilders.ToArray())
        {
        }

        private readonly IRuleExpressionBuilder[] _ruleExpressionBuilders;
        private readonly MethodInfo _anyMethod;

        /// <summary>
        /// Builds an expression for the rule set
        /// </summary>
        /// <param name="ruleSet">RuleSet</param>
        /// <param name="ruleSetContext">context</param>
        /// <returns>Expression</returns>
        public Expression BuildExpression(RuleSet ruleSet, RuleSetContext ruleSetContext)
        {
            if (ruleSet == null)
                throw new ArgumentNullException(nameof(ruleSet));

            if (!ruleSet.HasRulesOrRuleSets())
                throw new ArgumentException("Ruleset is not valid", nameof(ruleSet));

            if (ruleSetContext == null)
                throw new ArgumentNullException(nameof(ruleSetContext));

            if (ruleSetContext.ParameterExpression == null)
                throw new ArgumentException($"{nameof(ruleSetContext.ParameterExpression)} not set", nameof(ruleSetContext));

            if (ruleSetContext.PathWalker == null)
                throw new ArgumentException($"{nameof(ruleSetContext.PathWalker)} not set", nameof(ruleSetContext));

            var expressions = new List<Expression>();

            foreach (var innerRuleSet in ruleSet.RuleSets)
                expressions.Add(BuildExpression(innerRuleSet, ruleSetContext));
          
            //group rules by path so we don't have to keep re-building the logic to navigate the same path
            //also if there is a collection in the route then the rules have to share the same route and be applied via an Any method
            var rulesGroupedByPath = ruleSet.Rules.GroupBy(x => x.Path);

            foreach (var ruleGroup in rulesGroupedByPath)
            {
                var rulePathParts = ruleSetContext.GetPathParts(ruleGroup.Key);
                                 
                if (rulePathParts.Count() > 1)
                {
                    rulePathParts.Reverse();
                    Expression innerExpression = BuildExpressionToNavigateIEnumerableAndExecuteRules(rulePathParts, ruleGroup, ruleSet.LogicalOperator, ruleSetContext);
                    expressions.Add(innerExpression);
                }
                else
                {
                    var ruleBuilderContext = new RuleBuilderContext(ruleSetContext.ParameterExpression, rulePathParts[0].FinalType);

                    foreach (var rule in ruleGroup)
                        expressions.Add(BuildRuleExpression(rule, ruleBuilderContext));                    
                }
            }

            return CombineExpressionsByLogicalOperator(expressions, ruleSet.LogicalOperator);
        }

        /// <summary>
        /// Builds an expression to apply rules when the rule path(s) pass through one or more IEnumerables
        /// </summary> 
        /// <param name="rulePathPartsReversed"> A series of paths. The path to the rule(s) split at collections. reversed, so starts with the final part in the path.</param>
        /// <param name="rules">List of rules that use this path</param>
        /// <param name="logicalOperator">logical operator to combine the rules with</param>
        /// <param name="ruleSetContext">context</param>
        /// <returns>Expression for a lamda to navigate to and execute the rules</returns>
        private Expression BuildExpressionToNavigateIEnumerableAndExecuteRules(List<PathPart> rulePathPartsReversed, IEnumerable<Rule> rules, string logicalOperator, RuleSetContext ruleSetContext)
        {
            Expression innerExpression = null;

            for (var i = 0; i < rulePathPartsReversed.Count(); i++)
            {
                var pathPart = rulePathPartsReversed[i];
                var innerParameterExpression = Expression.Parameter(pathPart.SourceType, $"ie{i}");

                if (i == 0)               
                    innerExpression = BuildInnerRuleExecutionExpression(rules, logicalOperator, innerParameterExpression, pathPart);                
                else if (innerExpression != null)
                {
                    innerExpression = BuildExpressionToNavigatePathPartViaAny(ruleSetContext, pathPart, innerExpression);

                    if (i < rulePathPartsReversed.Count() - 1)
                        innerExpression = Expression.Lambda(innerExpression, innerParameterExpression);
                }
            }

            return innerExpression;
        }

        /// <summary>
        /// Builds an expression to execute a rule at the end of a path part
        /// </summary>
        /// <param name="rules">List of rules to execute</param>
        /// <param name="logicalOperator">Logical operator to combine rules with e.g. AND or OR</param>
        /// <param name="innerParameterExpression">Parameter Expression to access the path from. The Paramter expression will be for the source type of the path part</param>
        /// <param name="pathPart">PathPart to execute rules on</param>
        /// <returns>Expression</returns>
        private Expression BuildInnerRuleExecutionExpression(IEnumerable<Rule> rules, string logicalOperator, ParameterExpression innerParameterExpression, PathPart pathPart)
        {
            var innerRuleExpressions = new List<Expression>();

            Expression innerExpression;
            var ruleBuilderContext = new RuleBuilderContext(innerParameterExpression, pathPart.FinalType);

            foreach (var rule in rules)
            {
                var innerRule = new Rule
                {
                    ComparisonOperator = rule.ComparisonOperator,
                    Path = pathPart.Path,
                    Value = rule.Value,
                };

                innerRuleExpressions.Add(BuildRuleExpression(innerRule, ruleBuilderContext));
            }

            innerExpression = CombineExpressionsByLogicalOperator(innerRuleExpressions, logicalOperator);
            innerExpression = Expression.Lambda(innerExpression, innerParameterExpression);
            return innerExpression;
        }

        /// <summary>
        /// Builds an expression to navigate a path part up to an Any Method
        /// </summary>
        /// <param name="ruleSetContext">RuleSetContext</param>
        /// <param name="pathPart">PathPart to navigate</param>
        /// <param name="innerExpression">current expression to apply within the Any method</param>
        /// <returns>Expression</returns>
        private Expression BuildExpressionToNavigatePathPartViaAny(RuleSetContext ruleSetContext, PathPart pathPart, Expression innerExpression)
        {
            var propertyExpression = PropertyPath.AsPropertyExpression(pathPart.Path, ruleSetContext.ParameterExpression);
            var genericAnyMethod = _anyMethod.MakeGenericMethod(pathPart.FinalType.GetGenericArguments()[0]);
            innerExpression = Expression.Call(genericAnyMethod, propertyExpression, innerExpression);
            return innerExpression;
        }

        /// <summary>
        /// Builds an expression that combines a list of expressions with the given logical operator
        /// </summary>
        /// <param name="expressions">List of expressions</param>
        /// <param name="logicalOperator">logical operator (OR, AND)</param>
        /// <returns>Expression</returns>
        private static Expression CombineExpressionsByLogicalOperator(IList<Expression> expressions, string logicalOperator)
        {
            if (expressions.Count == 1)
                return expressions[0];

            Func<Expression, Expression, Expression> operatorFunction;

            if (logicalOperator.Equals(LogicalOperators.OR, StringComparison.InvariantCultureIgnoreCase))
                operatorFunction = (e1, e2) => Expression.OrElse(e1, e2);
            else if (logicalOperator.Equals(LogicalOperators.AND, StringComparison.InvariantCultureIgnoreCase))
                operatorFunction = (e1, e2) => Expression.AndAlso(e1, e2);
            else
                throw new NotSupportedException($"Logical Operator {logicalOperator} not supported");

            Expression finalExpression = operatorFunction(expressions[0], expressions[1]);

            for (var i = 2; i < expressions.Count; i++)
                finalExpression = operatorFunction(finalExpression, expressions[i]);

            return finalExpression;
        }

        /// <summary>
        /// Builds an expression for a rule
        /// </summary>
        /// <param name="rule">Rule to build expression for</param>
        /// <param name="ruleBuilderContext">RuleBuilderContext</param>
        /// <returns>Expression</returns>
        private Expression BuildRuleExpression(Rule rule, RuleBuilderContext ruleBuilderContext)
        {
            var expressionBuilder = _ruleExpressionBuilders.SingleOrDefault(x => x.CanInterpretRule(rule));

            if (expressionBuilder == null)
                throw new RuleExpressionBuilderNotFoundException(rule);

            return expressionBuilder.BuildRuleExpression(rule, ruleBuilderContext);
        }
    }
}
