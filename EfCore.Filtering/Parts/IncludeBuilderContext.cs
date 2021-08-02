using System;

namespace EfCore.Filtering.Parts
{
    /// <summary>
    /// context to use to help build Include Queries
    /// </summary>
    internal class IncludeBuilderContext : BuilderContext
    {
        /// <summary>
        /// For includes, after the first level of includes this will be set. Otherwise null 
        /// </summary>
        internal Type PreviousPropertyType { get; set; } 
    }

    /// <summary>
    /// Builds IncludeContexts
    /// </summary>
    internal static class IncludeBuilderContextBuilders
    {
        /// <summary>
        /// Converts an existing BuilderContext into and IncludeBuilderContext
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>IncludeBuilderContext</returns>
        public static IncludeBuilderContext ToIncludeBuilderContext(this BuilderContext context)
        {
            if (context is IncludeBuilderContext)
                return (IncludeBuilderContext)context;

            return new IncludeBuilderContext
            {
                CurrentExpression = context.CurrentExpression,
                Filter = context.Filter,
                PathWalker = context.PathWalker,
                SourceEntityType = context.SourceEntityType
            };
        }
    }
}
