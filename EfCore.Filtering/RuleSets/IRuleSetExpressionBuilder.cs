using EfCore.Filtering.Client;
using System;
using System.Linq.Expressions;

namespace EfCore.Filtering.RuleSets
{
    /// <summary>
    /// Defines how to interpret a ruleset and build an expression
    /// </summary>
    public interface IRuleSetExpressionBuilder
    {
        /// <summary>
        /// Builds an expression for the rule set
        /// </summary>
        /// <param name="ruleSet">RuleSet</param>
        /// <param name="ruleContext">context</param>
        /// <returns>Expression</returns>
        Expression BuildExpression(RuleSet ruleSet, RuleSetContext ruleContext);
    }
}
