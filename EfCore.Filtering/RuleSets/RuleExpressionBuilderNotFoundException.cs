using EfCore.Filtering.Client;
using System;
using System.Runtime.Serialization;
using System.Text.Json;

namespace EfCore.Filtering.RuleSets
{
    /// <summary>
    /// Exception for when no RuleExpressionBuilder can be found for the input
    /// </summary>
    public class RuleExpressionBuilderNotFoundException : Exception
    {
        public RuleExpressionBuilderNotFoundException(Rule rule)
            : base($"{message} Rule: {JsonSerializer.Serialize(rule)}")
        {
            Rule = rule;
        }

        const string message = "No IRuleExpressionBuilder found. This exception can also be thrown if there are multiple matching IRuleExpressionBuilders.";
        
        /// <summary>
        /// The rule that an expression can not be built for
        /// </summary>
        public Rule Rule { get; private set; }

        protected RuleExpressionBuilderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
