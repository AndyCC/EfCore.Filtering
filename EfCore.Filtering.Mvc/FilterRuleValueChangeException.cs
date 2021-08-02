using System;
using System.Runtime.Serialization;

namespace EfCore.Filtering.Mvc
{
    /// <summary>
    /// Exception thrown when a value on a rule can not be converted to the correct type
    /// </summary>
    public class FilterRuleValueChangeException : Exception
    {
        public FilterRuleValueChangeException()
        {
        }

        public FilterRuleValueChangeException(string message) : base(message)
        {
        }

        public FilterRuleValueChangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FilterRuleValueChangeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
