using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace EfCore.Filtering.RuleSets.Rules
{
    /// <summary>
    /// Builds expression for LIKE statements
    /// </summary>
    public class LikeRuleBuilder : IRuleExpressionBuilder
    {
        private static readonly Type _stringType = typeof(string);

        /// <summary>
        /// Builds a rule expression for a LIKE statement
        /// </summary>
        /// <param name="rule">rule to evaluate</param>
        /// <param name="context">Context containing items to build the rule with</param>
        /// <returns>Expression</returns>
        public Expression BuildRuleExpression(Rule rule, RuleBuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var propertyPathExpression = PropertyPath.AsPropertyExpression(rule.Path, context.ParameterExpression);
            var likeValueExpression = Expression.Constant(rule.Value);

            var dbFunctionsExpression =  Expression.Property(null, typeof(EF), nameof(EF.Functions));
            var method = typeof(DbFunctionsExtensions).GetMethod("Like", new Type[] { typeof(DbFunctions), _stringType, _stringType });
            return Expression.Call(method, dbFunctionsExpression, propertyPathExpression, likeValueExpression);
        }

        /// <summary>
        /// Determines if a rule can be converted to a LIKE statement
        /// </summary>
        /// <param name="rule">Rule to interpret</param>
        /// <returns>true if can interpret, otherwise false</returns>
        public bool CanInterpretRule(Rule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            return rule.ComparisonOperator.Equals("Like", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
