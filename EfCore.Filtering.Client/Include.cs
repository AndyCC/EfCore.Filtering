using System;
using System.Linq.Expressions;

namespace EfCore.Filtering.Client
{
    /// <summary>
    /// An Include
    /// </summary>
    public class Include
    {
        /// <summary>
        /// Object path from source entity to reach the property to include
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The filter to use with the include, leave null when include does not require a filter
        /// </summary>
        public Filter Filter { get; set; }
    }

    /// <summary>
    /// An Include
    /// </summary>
    /// <typeparam name="TSource">source type to apply rule to</typeparam>
    public class Include<TSource> : Include
    {
        /// <summary>
        /// Epression to set the path
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
