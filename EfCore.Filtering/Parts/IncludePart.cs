using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCore.Filtering.Parts
{
    /// <summary>
    /// Adds includes to query
    /// </summary>
    public class IncludePart : IBuilderPart
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="includeFilteringParts">list of parts used to build expressions for the include filters</param>
        public IncludePart(params IIncludeFilteringPart[] includeFilteringParts)
            : this ((IEnumerable<IIncludeFilteringPart>) includeFilteringParts)
        {           
        }

        public IncludePart(IEnumerable<IIncludeFilteringPart> includeFilteringParts)      
        {
            _includeFilteringParts = includeFilteringParts.OrderBy(x => x.ExecutionOrder).ToArray();

            var queryableType = typeof(IQueryable<>);
            var expressionFunc2Type = new ParameterTypeInfo(typeof(Expression<>), typeof(Func<,>));
            var includeQueryable2Type = new ParameterTypeInfo(typeof(IIncludableQueryable<,>), new ParameterTypeInfo(null), new ParameterTypeInfo(null));
            var includeQueryable2WithEnumerableType = new ParameterTypeInfo(typeof(IIncludableQueryable<,>), new ParameterTypeInfo(null), typeof(IEnumerable<>));

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            _includeMethod = typeof(EntityFrameworkQueryableExtensions).GetGenericMethod("Include", bindingFlags, queryableType, expressionFunc2Type);
            _thenIncludeMethod = typeof(EntityFrameworkQueryableExtensions).GetGenericMethod("ThenInclude", bindingFlags, includeQueryable2Type, expressionFunc2Type);
            _thenIncludeEnumerableMethod = typeof(EntityFrameworkQueryableExtensions).GetGenericMethod("ThenInclude", bindingFlags, includeQueryable2WithEnumerableType, expressionFunc2Type);
        }

        private readonly IIncludeFilteringPart[] _includeFilteringParts;

        private readonly MethodInfo _includeMethod;
        private readonly MethodInfo _thenIncludeMethod;
        private readonly MethodInfo _thenIncludeEnumerableMethod;

        /// <summary>
        /// Part execution order position
        /// </summary>
        public int ExecutionOrder { get; set; } = 500;

        /// <summary>
        /// Builds include part of an expression
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>updated expression</returns>
        public Expression BuildExpression(BuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.IsValid())
                throw new ArgumentNullException(nameof(context), "input is not valid");

            return BuildExpressionInternal(context.ToIncludeBuilderContext());
        }

        /// <summary>
        ///  Builds include part of an expression
        /// </summary>
        /// <param name="context">IncludeBuilderContext</param>
        /// <returns>Expression</returns>
        private Expression BuildExpressionInternal(IncludeBuilderContext context)
        {
            foreach (var include in context.Filter.Includes)
            {
                var sourceType = context.SourceEntityType.GetUnderlyingTypeIfGenericAndEnumerable() ?? context.SourceEntityType;
                var parameterExpression = Expression.Parameter(sourceType, "b");
                var propertyPathExpression = PropertyPath.AsPropertyExpression(include.Path, parameterExpression);
                var propertyType = context.PathWalker.GetFinalTypeOfFinalPropertyInPath(sourceType, include.Path);

                if (propertyType.IsAssignableTo(typeof(IEnumerable)))
                    context.CurrentExpression = AddIncludeWhenAList(context, propertyPathExpression, propertyType, include.Filter, parameterExpression);
                else
                    context.CurrentExpression = AddIncludeWhenNotAList(context, propertyPathExpression, propertyType, parameterExpression);

                if (include.Filter != null)
                    AddInnerIncludes(context, include.Filter, propertyType);
            }

            return context.CurrentExpression;
        }

        /// <summary>
        /// Adds an include expression when the targeted property implements IEnumerable
        /// </summary>
        /// <param name="context">IncludeBuilderContext</param>
        /// <param name="propertyPathExpression">expression to get property</param>
        /// <param name="propertyType">type of property retrieved by propertyPathExpression</param>
        /// <param name="includeFilter">filter for the include</param>
        /// <param name="parameterExpression">expression for the parameter of the source type for the include</param>
        private Expression AddIncludeWhenAList(IncludeBuilderContext context, Expression propertyPathExpression, Type propertyType, Filter includeFilter, ParameterExpression parameterExpression)
        {
            Expression includeListExpression;

            if (includeFilter.CanApplyAsIncludeFilter())
            {
                var includeContext = new BuilderContext
                {
                    CurrentExpression = propertyPathExpression,
                    SourceEntityType = propertyType.GetUnderlyingTypeIfGenericAndEnumerable() ?? propertyType,
                    Filter = includeFilter,
                    PathWalker = context.PathWalker,
                };

                foreach (var part in _includeFilteringParts)
                    includeContext.CurrentExpression = part.BuildIncludeExpression(includeContext);

                includeListExpression = includeContext.CurrentExpression;
            }
            else
                includeListExpression = propertyPathExpression;

            MethodInfo method = IncludeMethod(context, includeListExpression.Type);
            var lamdaExpression = Expression.Lambda(includeListExpression, parameterExpression);
            return Expression.Call(method, context.CurrentExpression, lamdaExpression);
        }

        /// <summary>
        /// Adds and include when the propertytype does not implement IEnumerable
        /// </summary>
        /// <param name="context">IncludeBuilderContext</param>
        /// <param name="propertyPathExpression">expression to get property</param>
        /// <param name="propertyType">type of property retrieved by propertyPathExpression</param>
        /// <param name="parameterExpression">expression for the parameter of the source type for the include</param>
        private Expression AddIncludeWhenNotAList(IncludeBuilderContext context, Expression propertyPathExpression, Type propertyType, ParameterExpression parameterExpression)
        {
            MethodInfo method = IncludeMethod(context, propertyType);
            var lamdaExpression = Expression.Lambda(propertyPathExpression, parameterExpression);
            return Expression.Call(method, context.CurrentExpression, lamdaExpression);
        }

        /// <summary>
        /// Returns the include or then include method to use based on the previous property type and source property type in the context and the current property type being targeted
        /// </summary>
        /// <param name="context">IncludeBuilderContext</param>
        /// <param name="propertyType">type of the property being targeted</param>
        /// <returns>MethodInfo</returns>
        private MethodInfo IncludeMethod(IncludeBuilderContext context, Type propertyType)
        {
            if (context.PreviousPropertyType != null)
                return ThenIncludeMethod(context, propertyType);
            else 
                return _includeMethod.MakeGenericMethod(context.SourceEntityType, propertyType);
        }

        /// <summary>
        /// Returns the then include method to use based on the previous property type and source property type in the context and the current property type being targeted
        /// </summary>
        /// <param name="context">IncludeBuilderContext</param>
        /// <param name="propertyType">type of the property being targeted</param>
        /// <returns>MethodInfo</returns>
        private MethodInfo ThenIncludeMethod(IncludeBuilderContext context, Type propertyType)
        {
            if (context.SourceEntityType.IsAssignableTo(typeof(IEnumerable)))
            {
                var sourceType = context.SourceEntityType.GetUnderlyingTypeIfGenericAndEnumerable() ?? context.SourceEntityType;
                return _thenIncludeEnumerableMethod.MakeGenericMethod(context.PreviousPropertyType, sourceType, propertyType);
            }
            else
                return _thenIncludeMethod.MakeGenericMethod(context.PreviousPropertyType, context.SourceEntityType, propertyType);
        }

        /// <summary>
        /// Adds inner includes thay are found on an includes filter
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <param name="includeFilter">Filter used by the current include</param>
        /// <param name="propertyType">Type of property returned by the current include</param>
        private void AddInnerIncludes(BuilderContext context, Filter includeFilter, Type propertyType)
        {
            if (!context.Filter.HasIncludes)
                return;

            var innerContext = new IncludeBuilderContext
            {
                CurrentExpression = context.CurrentExpression,
                Filter = includeFilter,
                SourceEntityType = propertyType,
                PreviousPropertyType = context.SourceEntityType,
                PathWalker = context.PathWalker
            };

            context.CurrentExpression = BuildExpression(innerContext);
        }
    }
}
