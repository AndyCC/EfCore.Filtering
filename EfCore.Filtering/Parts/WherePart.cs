using EfCore.Filtering.RuleSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCore.Filtering.Parts
{
    /// <summary>
    /// Adds where to the main query or to the filtering on an include
    /// </summary>
    public class WherePart : IBuilderPart, IIncludeFilteringPart
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="ruleSetExpressionBuilder">IRuleSetExpressionBuilder tells the Part how to build expressions for each rule type</param>
        public WherePart(IRuleSetExpressionBuilder ruleSetExpressionBuilder)
        {
            _ruleSetExpressionBuilder = ruleSetExpressionBuilder;

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            var expressionFunc2Type = new ParameterTypeInfo(typeof(Expression<>), typeof(Func<,>));

            _whereQueryableMethod = typeof(Queryable).GetGenericMethod("Where", bindingFlags, typeof(IQueryable<>), expressionFunc2Type);
            _whereEnumerableMethod = typeof(Enumerable).GetGenericMethod("Where", bindingFlags, typeof(IEnumerable<>), typeof(Func<,>));
        }

        private readonly IRuleSetExpressionBuilder _ruleSetExpressionBuilder;
        private readonly MethodInfo _whereQueryableMethod;
        private readonly MethodInfo _whereEnumerableMethod;

        /// <summary>
        /// Part execution order position
        /// </summary>
        public int ExecutionOrder { get; set; } = 0;

        /// <summary>
        /// Adds Where to the expression
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>Expresion original expression with the where expression added</returns>
        public Expression BuildExpression(BuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.IsValid())
                throw new ArgumentNullException(nameof(context), "input is not valid");

            return BuildTheExpression(context, _whereQueryableMethod);
        }

        /// <summary>
        /// Adds Where to the expression for an include
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <returns>Expresion original expression with the where expression added</returns>
        public Expression BuildIncludeExpression(BuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.IsValid())
                throw new ArgumentNullException(nameof(context), "input is not valid");

            return BuildTheExpression(context, _whereEnumerableMethod);
        }

        /// <summary>
        /// Common method to build an expression to create the where clause
        /// </summary>
        /// <param name="context">BuilderContext</param>
        /// <param name="whereMethod">MethodInfo of the where method to use</param>
        /// <returns>Expresion original expression with the where expression added</returns>
        private Expression BuildTheExpression(BuilderContext context, MethodInfo whereMethod)
        {   
            var filter = context.Filter;

            if (filter.WhereClause == null || !filter.WhereClause.HasRulesOrRuleSets())
                return context.CurrentExpression;

            var parameterExpression = Expression.Parameter(context.SourceEntityType, "x");
            var whereExpression = _ruleSetExpressionBuilder.BuildExpression(filter.WhereClause, new RuleSetContext(parameterExpression, context.PathWalker));
            var lamdaExpression = Expression.Lambda(whereExpression, parameterExpression);

            var method = whereMethod.MakeGenericMethod(context.SourceEntityType);
            context.CurrentExpression = Expression.Call(method, context.CurrentExpression, lamdaExpression);
            return context.CurrentExpression;
        }
    }
}
