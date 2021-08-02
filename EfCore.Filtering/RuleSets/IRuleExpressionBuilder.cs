using EfCore.Filtering.Client;
using System.Linq.Expressions;

namespace EfCore.Filtering.RuleSets
{
    /// <summary>
    /// Defines how to interpret a rule and build an expression
    /// </summary>
    public interface IRuleExpressionBuilder
    {
        /// <summary>
        /// Builds a rule expression
        /// </summary>
        /// <param name="rule">Rule to evaluate</param>
        /// <param name="context">Context containing items to build the rule with</param>
        /// <returns>Expression</returns>
        public Expression BuildRuleExpression(Rule rule, RuleBuilderContext context);

        /// <summary>
        /// Determines if a rule can be intepreted based on the operator
        /// </summary>
        /// <param name="rule">Rule to interpret</param>
        /// <returns>true if can interpret, otherwise false</returns>
        public bool CanInterpretRule(Rule rule);

    }
}
