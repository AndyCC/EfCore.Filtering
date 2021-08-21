using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EfCore.Filtering.RuleSets.Rules
{
    /// <summary>
    /// Creates expression for simple comparisons:
    /// EqualTo,
    /// NotEqual,
    /// GreaterThan,
    /// GreaterThanOrEqual,
    /// LessThan,
    /// LessThanOrEqual
    /// </summary>
    public class SimpleComparisonRuleBuilder : IRuleExpressionBuilder
    {
        private readonly static string[] _operatorsInterpreted = new string[]
        {
            "Equal",
            "Eq",
            "NotEqual",
            "Ne",
            "GreaterThan",
            "Gt",
            "GreaterThanOrEqual",
            "Gte",
            "LessThan",
            "Lt",
            "LessThanOrEqual",
            "Lte"
        };

        /// <summary>
        /// Builds a rule expression for a comparison
        /// </summary>   
        /// <param name="rule">rule to evaluate</param>
        /// <param name="context">Context containing items to build the rule with</param>
        /// <returns>Expression</returns>
        public Expression BuildRuleExpression(Rule rule, RuleBuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var propertyPathExpression = PropertyPath.AsPropertyExpression(rule.Path, context.ParameterExpression);
            var constantExpression = Expression.Constant(rule.Value);

            return rule.ComparisonOperator.ToLower() switch
            {
                "equal" or "eq" => Expression.Equal(propertyPathExpression, constantExpression),
                "notequal" or "ne" => Expression.NotEqual(propertyPathExpression, constantExpression),
                "greaterthan" or "gt" => Expression.GreaterThan(propertyPathExpression, constantExpression),
                "greaterthanorequal" or "gte" => Expression.GreaterThanOrEqual(propertyPathExpression, constantExpression),
                "lessthan" or "lt" => Expression.LessThan(propertyPathExpression, constantExpression),
                "lessthanorequal" or "lte" => Expression.LessThanOrEqual(propertyPathExpression, constantExpression),
                _ => throw new NotImplementedException($"Comparison operator {rule.ComparisonOperator} not implemented"),
            };
        }

        /// <summary>
        /// Determines if a rule can be converted to a comparison statement
        /// </summary>
        /// <param name="rule">Rule to interpret</param>
        /// <returns>true if can interpret, otherwise false</returns>
        public bool CanInterpretRule(Rule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            return _operatorsInterpreted.Any(x => x.Equals(rule.ComparisonOperator, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
