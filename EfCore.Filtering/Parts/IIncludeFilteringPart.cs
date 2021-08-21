using System.Linq.Expressions;

namespace EfCore.Filtering.Parts
{
    /// <summary>
    /// Builds an expression tree to represent logic to be included in the QueryBuilder's Include part. 
    /// </summary>
    public interface IIncludeFilteringPart
    {
        /// <summary>
        /// determines when part will be executed. Lower numbers execute first
        /// </summary>
        public int ExecutionOrder { get; set; }

        /// <summary>
        /// Builds part of an expression for an include's filter
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>updated expression</returns>
        public Expression BuildIncludeExpression(BuilderContext context);
    }
}
