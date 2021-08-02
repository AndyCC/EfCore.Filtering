using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCore.Filtering.Parts
{
    /// <summary>
    /// Adds take to the main query or to the filtering on an include
    /// </summary>
    public class TakePart : IBuilderPart, IIncludeFilteringPart
    { 
        /// <summary>
        /// Ctor
        /// </summary>
        public TakePart()
        {
            _queryableTake = typeof(Queryable).GetMethod("Take", BindingFlags.Public | BindingFlags.Static);
            _enumerableTake = typeof(Enumerable).GetMethod("Take", BindingFlags.Public | BindingFlags.Static);
        }

        private readonly MethodInfo _queryableTake;
        private readonly MethodInfo _enumerableTake;

        /// <summary>
        /// Part execution order position
        /// </summary>
        public int ExecutionOrder { get; set; } = 2000;

        /// <summary>
        /// Adds Take to the expression
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>Expresion original expression with the take expression added</returns>
        public Expression BuildExpression(BuilderContext context)
        {
            if (!context.IsValid())
                throw new ArgumentNullException(nameof(context), "input is not valid");

            return BuildTheExpression(context, _queryableTake);
        }

        /// <summary>
        /// Adds Take to the include expression
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>Expresion original expression with the take expression added</returns>
        public Expression BuildIncludeExpression(BuilderContext context)
        {
            return BuildTheExpression(context, _enumerableTake);
        }

        /// <summary>
        /// Common method to build the take expression
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <param name="takeMethod">MethodInfo for the method to use for Take</param>
        /// <returns>Expresion original expression with the take expression added</returns>
        private static Expression BuildTheExpression(BuilderContext context, MethodInfo takeMethod)
        {
            if (context.Filter.Take.HasValue)
            {
                var genericMethod = takeMethod.MakeGenericMethod(context.SourceEntityType);
                var takeValueExpression = Expression.Constant(context.Filter.Take.Value);
                context.CurrentExpression = Expression.Call(genericMethod, context.CurrentExpression, takeValueExpression);
            }

            return context.CurrentExpression;
        }
    }
}
