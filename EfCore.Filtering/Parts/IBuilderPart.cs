using System.Linq.Expressions;

namespace EfCore.Filtering.Parts
{
    /// <summary>
    /// Logic to be included in the QueryBuilder
    /// </summary>
    public interface IBuilderPart
    {
        /// <summary>
        /// determines when part will be executed. Lower numbers execute first
        /// </summary>
        public int ExecutionOrder { get; set; }

        /// <summary>
        /// Builds part of an expression
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>updated expression</returns>
        public Expression BuildExpression(BuilderContext context);
    }
}
