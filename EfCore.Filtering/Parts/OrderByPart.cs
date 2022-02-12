using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCore.Filtering.Parts
{
    /// <summary>
    /// adds order by to the main query or to filtering on an include
    /// </summary>
    public class OrderByPart : IBuilderPart, IIncludeFilteringPart
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public OrderByPart()
        {
            var queryableType = typeof(IQueryable<>);
            var expressionFunc2Type = new ParameterTypeInfo(typeof(Expression<>), new ParameterTypeInfo(typeof(Func<,>)));
            var orderedQueryableType = typeof(IOrderedQueryable<>);
            var enumerableType = typeof(IEnumerable<>);
            var orderedEnumerableType = typeof(IOrderedEnumerable<>);
            var func2Type = typeof(Func<,>);

            const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;

            _queryableOrderByMethod = typeof(Queryable).GetGenericMethod("OrderBy", bindingFlags, queryableType, expressionFunc2Type);
            _queryableOrderByDescMethod = typeof(Queryable).GetGenericMethod("OrderByDescending", bindingFlags, queryableType, expressionFunc2Type);
            _queryableThenByMethod = typeof(Queryable).GetGenericMethod("ThenBy", bindingFlags, orderedQueryableType, expressionFunc2Type);
            _queryableThenByDescMethod = typeof(Queryable).GetGenericMethod("ThenByDescending", bindingFlags, orderedQueryableType, expressionFunc2Type);

            _enumerableOrderByMethod = typeof(Enumerable).GetGenericMethod("OrderBy", bindingFlags, enumerableType, func2Type);
            _enumerableOrderByDescMethod = typeof(Enumerable).GetGenericMethod("OrderByDescending", bindingFlags, enumerableType, func2Type);
            _enumerableThenByMethod = typeof(Enumerable).GetGenericMethod("ThenBy", bindingFlags, orderedEnumerableType, func2Type);
            _enumerableThenByDescMethod = typeof(Enumerable).GetGenericMethod("ThenByDescending", bindingFlags, orderedEnumerableType, func2Type);

            _singleMethod = typeof(Enumerable).GetGenericMethod("Single", bindingFlags, enumerableType);
        }

        private readonly MethodInfo _queryableOrderByMethod;
        private readonly MethodInfo _queryableOrderByDescMethod;
        private readonly MethodInfo _queryableThenByMethod;
        private readonly MethodInfo _queryableThenByDescMethod;

        private readonly MethodInfo _enumerableOrderByMethod;
        private readonly MethodInfo _enumerableOrderByDescMethod;
        private readonly MethodInfo _enumerableThenByMethod;
        private readonly MethodInfo _enumerableThenByDescMethod;

        private readonly MethodInfo _singleMethod;

        /// <summary>
        /// Part execution order position
        /// </summary>
        public int ExecutionOrder { get; set; } = 1000;

        /// <summary>
        /// Adds an expression for the order by if the ordering has been set on the filter
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>Expresion original expression with the order by expression added</returns>
        public Expression BuildExpression(BuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.IsValid())
                throw new ArgumentNullException(nameof(context), "input is not valid");

            return BuildTheExpression(context, _queryableOrderByMethod, _queryableOrderByDescMethod, _queryableThenByMethod, _queryableThenByDescMethod);
        }

        /// <summary>
        /// Builds expression when part of an Include
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>Expresion original expression with the order by expression added</returns>
        public Expression BuildIncludeExpression(BuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.IsValid())
                throw new ArgumentNullException(nameof(context), "input is not valid");

            return BuildTheExpression(context, _enumerableOrderByMethod, _enumerableOrderByDescMethod, _enumerableThenByMethod, _enumerableThenByDescMethod);
        }

        /// <summary>
        /// Common method to build the order by expresion required to fulfill all ordering provided in the filter
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <param name="orderByMethod">MethodInfo for order by method to use in the expression, if required</param>
        /// <param name="orderByDescMethod">MethodInfo for order by descending method to use in the expression, if required</param>
        /// <param name="thenByMethod">MethodInfo for then by method to use in the expression, if required</param>
        /// <param name="thenByDescMethod">MethodInfo for then by descending method to use in the expression, if required</param>
        /// <returns>Expresion original expression with the order by expression added</returns>
        private Expression BuildTheExpression(BuilderContext context, MethodInfo orderByMethod, MethodInfo orderByDescMethod, MethodInfo thenByMethod, MethodInfo thenByDescMethod)
        {
            var filter = context.Filter;

            if (filter.Ordering == null || filter.Ordering.Count == 0)
                return context.CurrentExpression;

            bool isFirstOrderBy = true;

            foreach (var orderBy in filter.Ordering)
            {
                var pathParts = context.PathWalker.PathParts(context.SourceEntityType, orderBy.Path).ToList();
                Expression expression = BuildExpressionToNavigateToPropertyToOrder(context.SourceEntityType, orderBy, pathParts);

                MethodInfo method;
                var isAscending = orderBy.Order.StartsWith("asc", StringComparison.InvariantCultureIgnoreCase);
                var isDescending = orderBy.Order.StartsWith("desc", StringComparison.InvariantCultureIgnoreCase);

                if (!isAscending && !isDescending)
                    throw new InvalidOperationException($"invalid order by {orderBy.Order}");

                var propertyType = pathParts[pathParts.Count - 1].FinalType;
                if (isFirstOrderBy)
                {
                    if (isAscending)
                        method = orderByMethod.MakeGenericMethod(context.SourceEntityType, propertyType);
                    else
                        method = orderByDescMethod.MakeGenericMethod(context.SourceEntityType, propertyType);
                }
                else
                {
                    if (isAscending)
                        method = thenByMethod.MakeGenericMethod(context.SourceEntityType, propertyType);
                    else
                        method = thenByDescMethod.MakeGenericMethod(context.SourceEntityType, propertyType);
                }

                context.CurrentExpression = Expression.Call(method, context.CurrentExpression, expression);
                isFirstOrderBy = false;
            }

            return context.CurrentExpression;
        }

        /// <summary>
        /// Builds an expression to navigate the path to the property that is to be ordered on
        /// </summary>
        /// <param name="sourceType">Type of the first object in the path</param>
        /// <param name="orderBy">ordering to apply to any collections on the path</param>
        /// <param name="pathParts">A list of paths, split at Enumerable Properties (e.g. path A.B.List1.C.D is split into [0] = A.B.List1 [1] = C.D)</param>
        /// <returns>Expression for a Lamda to navigate to the required property to order on</returns>
        private Expression BuildExpressionToNavigateToPropertyToOrder(Type sourceType, OrderBy orderBy, List<PathPart> pathParts)
        {
            ParameterExpression parameterExpression = null;
            Expression expression = null;

            for (var i = 0; i < pathParts.Count(); i++)
            {
                if (i == 0)
                {
                    parameterExpression = Expression.Parameter(sourceType, $"x{i}");
                    expression = PropertyPath.AsPropertyExpression(pathParts[i].Path, parameterExpression);                   
                }

                if (i < pathParts.Count-2)
                {
                    var genericType = pathParts[i].FinalType.GetUnderlyingTypeIfGenericAndEnumerable() ?? pathParts[i].FinalType;
                    var singleMethod = _singleMethod.MakeGenericMethod(genericType);
                    expression = Expression.Call(singleMethod, expression);
                }
                else if (i == pathParts.Count - 1 && pathParts.Count > 1)
                {
                    var isAscending = orderBy.Order.StartsWith("asc", StringComparison.InvariantCultureIgnoreCase);

                    MethodInfo method;
                    if (isAscending)
                        method = _enumerableOrderByMethod.MakeGenericMethod(pathParts[i].SourceType, pathParts[i].FinalType);
                    else
                        method = _enumerableOrderByDescMethod.MakeGenericMethod(pathParts[i].SourceType, pathParts[i].FinalType);
                    
                    var innerParameterExpression = Expression.Parameter(pathParts[i].SourceType, $"x{i}");
                    var propertyPathExpression = PropertyPath.AsPropertyExpression(pathParts[i].Path, innerParameterExpression);
                    var lamdaExpression = Expression.Lambda(propertyPathExpression, innerParameterExpression);

                    expression = Expression.Call(method, expression, lamdaExpression);
                    var singleMethod = _singleMethod.MakeGenericMethod(pathParts[i].SourceType);
                    expression = Expression.Call(singleMethod, expression);
                    expression = PropertyPath.AsPropertyExpression(pathParts[i].Path, expression);
                }
                else if(i > 0 && pathParts.Count > 1)
                {
                    expression = PropertyPath.AsPropertyExpression(pathParts[i].Path, expression);
                }
            }

            return Expression.Lambda(expression, parameterExpression); 
        }
    }
}
