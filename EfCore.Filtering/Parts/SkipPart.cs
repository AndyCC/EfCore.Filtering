using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCore.Filtering.Parts
{
    /// <summary>
    /// Adds skip to the main query or to the filtering on an include
    /// </summary>
    public class SkipPart : IBuilderPart, IIncludeFilteringPart
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public SkipPart()
        {
            _queryableSkip = typeof(Queryable).GetMethod("Skip", BindingFlags.Public | BindingFlags.Static); 
            _enumerableSkip = typeof(Enumerable).GetMethod("Skip", BindingFlags.Public | BindingFlags.Static);
        }

        private readonly MethodInfo _queryableSkip;
        private readonly MethodInfo _enumerableSkip;

        /// <summary>
        /// Part execution order position
        /// </summary>
        public int ExecutionOrder { get; set; } = 1500;

        /// <summary>
        /// Adds any skip to the Expression
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>Expresion original expression with the skip expression added</returns>
        public Expression BuildExpression(BuilderContext context)
        {
            if (!context.IsValid())
                throw new ArgumentNullException(nameof(context), "input is not valid");

            return BuildTheExpression(context, _queryableSkip);
        }

        /// <summary>
        /// Adds any skip to the Include Expression
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>Expresion original expression with the skip expression added</returns>
        public Expression BuildIncludeExpression(BuilderContext context)
        {
            if (!context.IsValid())
                throw new ArgumentNullException(nameof(context), "input is not valid");

            return BuildTheExpression(context, _enumerableSkip);
        }

        /// <summary>
        /// Common method to build the skip part of an expression
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <param name="skipMethod">MethodInfo for the skip method to use</param>
        /// <returns>Expresion original expression with the skip expression added</returns>
        private static Expression BuildTheExpression(BuilderContext context, MethodInfo skipMethod)
        {
            if (context.Filter.Skip.HasValue)
            {
                var genericMethod = skipMethod.MakeGenericMethod(context.SourceEntityType);
                var skipValueExpression = Expression.Constant(context.Filter.Skip.Value);
                context.CurrentExpression = Expression.Call(genericMethod, context.CurrentExpression, skipValueExpression);
            }

            return context.CurrentExpression;
        }
    }
}
