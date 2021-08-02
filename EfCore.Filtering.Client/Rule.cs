using System;
using System.Linq.Expressions;

namespace EfCore.Filtering.Client
{
    /// <summary>
    /// A filtering rule
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// Object path from source entity to reach the property to evaluate
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Comparison operator for the rule
        /// </summary>
        public string ComparisonOperator { get; set; }

        /// <summary>
        /// Value to evaluate against
        /// </summary>
        public Object Value { get; set; }
    }

    /// <summary>
    /// A filtering rule
    /// </summary>
    /// <typeparam name="TSource">source type to apply rule to</typeparam>
    public class Rule<TSource> : Rule
    {
        /// <summary>
        /// Sets the Path property using an expression
        /// </summary>
        public Expression<Func<TSource, object>> PathExpression
        {
            set
            {
                var pathParts = PropertyPath.GetPathParts<TSource, object>(value);
                Path = string.Join(".", pathParts);
            }
        }
    }
}
