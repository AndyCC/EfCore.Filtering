﻿using System;
using System.Linq;

namespace EfCore.Filtering.Client
{
    /// <summary>
    /// A rule set with shorted property names for use in a query string
    /// </summary>
    public class ShortRuleSetQueryString
    {
        /// <summary>
        /// Logical Operator (AND (A) - OR (O))
        /// </summary>
        public string L { get; set; }

        /// <summary>
        /// RuleSets
        /// </summary>
        public ShortRuleSetQueryString[] S { get; set; }

        /// <summary>
        /// Rules
        /// </summary>
        public ShortRuleQueryString[] R { get; set; }

        /// <summary>
        /// Creates a RuleSet
        /// </summary>
        /// <returns>RuleSet</returns>
        public RuleSet ToRuleSet()
        {
            var ruleSet = new RuleSet();

            if (L.Equals("O", StringComparison.OrdinalIgnoreCase) || L.Equals(LogicalOperators.OR, StringComparison.OrdinalIgnoreCase))
                ruleSet.LogicalOperator = LogicalOperators.OR;
            else if (L.Equals("A", StringComparison.OrdinalIgnoreCase) || L.Equals(LogicalOperators.AND, StringComparison.OrdinalIgnoreCase))
                ruleSet.LogicalOperator = LogicalOperators.AND;
            
            if (S != null)
                ruleSet.RuleSets = S.Select(x => x.ToRuleSet()).ToList();

            if (R != null)
                ruleSet.Rules = R.Select(x => x.ToRule()).ToList();

            return ruleSet;
        }
    }
}
