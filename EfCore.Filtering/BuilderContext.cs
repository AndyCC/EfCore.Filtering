using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using System;
using System.Linq.Expressions;

namespace EfCore.Filtering
{
    /// <summary>
    /// Context used for building the query
    /// </summary>
    public class BuilderContext
    {
        /// <summary>
        /// The expression that has been built so far
        /// </summary>        
        public Expression CurrentExpression { get; set; }

        /// <summary>
        /// The filter being applied
        /// </summary>
        public Filter Filter { get;  set; }

        /// <summary>
        /// Source Entity Type for use with the Paths in the filter
        /// </summary>
        public Type SourceEntityType { get; set; }

        /// <summary>
        /// PathWalker containing details of all the paths in the filter
        /// </summary>
        public PathWalker PathWalker { get; internal set; }

        /// <summary>
        /// Determines if the context is valid or not
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsValid()
        {
            return CurrentExpression != null &&
                Filter != null &&
                SourceEntityType != null;
        }
    }
}
