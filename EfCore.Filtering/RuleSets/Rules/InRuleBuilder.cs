using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCore.Filtering.RuleSets.Rules
{
    /// <summary>
    /// Builds expression for IN statements
    /// </summary>
    public class InRuleBuilder : IRuleExpressionBuilder
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public InRuleBuilder()
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;
            _containsMethod = typeof(Enumerable).GetGenericMethod("Contains", bindingFlags, typeof(IEnumerable<>), new ParameterTypeInfo(null));
        }

        private readonly MethodInfo _containsMethod;

        /// <summary>
        /// Builds a rule expression for an IN statement
        /// </summary>
        /// <param name="rule">rule to evaluate</param>
        /// <param name="context">Context containing items to build the rule with</param>
        /// <returns>Expression</returns>
        public Expression BuildRuleExpression(Rule rule, RuleBuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var propertyPathExpression = PropertyPath.AsPropertyExpression(rule.Path, context.ParameterExpression);
            var listExpression = Expression.Constant(rule.Value);

            var genericMethod = _containsMethod.MakeGenericMethod(context.TargetPropertyType);
            return Expression.Call(genericMethod, listExpression, propertyPathExpression);
        }

        /// <summary>
        /// Determines if a rule can be converted to an IN statement
        /// </summary>
        /// <param name="rule">Rule to interpret</param>
        /// <returns>true if can interpret, otherwise false</returns>
        public bool CanInterpretRule(Rule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            return rule.ComparisonOperator.Equals("IN", StringComparison.InvariantCultureIgnoreCase) &&
                rule.Value.GetType().IsAssignableTo(typeof(IEnumerable));
        }
    }
}
