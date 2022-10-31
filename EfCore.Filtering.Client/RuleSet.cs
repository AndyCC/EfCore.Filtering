using System;
using System.Collections.Generic;

namespace EfCore.Filtering.Client
{
    /// <summary>
    /// A filtering rule set
    /// Combines a number or rules and rulesets with the given logical operator
    /// </summary>
    public class RuleSet
    {
        private string _logicalOperator;

        /// <summary>
        /// Logical Operator to combine rules and inner rule sets
        /// </summary>
        public string LogicalOperator
        {
            get { return _logicalOperator; }
            set
            {
                if(!value.Equals(LogicalOperators.AND, StringComparison.InvariantCultureIgnoreCase) &&
                    !value.Equals(LogicalOperators.OR, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new NotSupportedException($"Logical operator '{value}' not supported");
                }

                _logicalOperator = value;
            }
        }

        /// <summary>
        /// List of rules
        /// </summary>
        public List<Rule> Rules { get; set; } = new List<Rule>();

        /// <summary>
        /// List of rule sets
        /// </summary>
        /// <remarks>
        /// This will be an additional set of rules within parenthesis.
        /// So if the current ruleset is combined with AND and the inner ruleset is combined with OR then the result will be something like
        /// (Rule1 AND (InnerRuleSet1.RuleA OR InnerRuleSet1.RuleB))
        /// </remarks>
        public List<RuleSet> RuleSets { get; set; } = new List<RuleSet>();


        /// <summary>
        /// Determines if this ruleset has rules or rulesets
        /// </summary>
        /// <returns>true if has rules or rulesets</returns>
        public bool HasRulesOrRuleSets()
        {
            return (Rules != null && Rules.Count > 0) || 
                (RuleSets != null && RuleSets.Count > 0);

        }
    }
}
