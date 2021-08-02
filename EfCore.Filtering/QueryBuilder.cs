using EfCore.Filtering.Client;
using EfCore.Filtering.Parts;
using EfCore.Filtering.Paths;
using EfCore.Filtering.RuleSets;
using EfCore.Filtering.RuleSets.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EfCore.Filtering
{
    /// <summary>
    /// QueryBuilder implementation 
    /// </summary>
    public class QueryBuilder : IQueryBuilder
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="builderParts">list of IBuilderPart to use to build the query</param>
        public QueryBuilder(params IBuilderPart[] builderParts)
        {
            if (builderParts == null)
                throw new ArgumentNullException(nameof(builderParts));

            if (builderParts.Length == 0)
                throw new ArgumentException("Must have at least 1", nameof(builderParts));

            _builderParts = builderParts.OrderBy(x => x.ExecutionOrder).ToArray(); 
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="builderParts">list of IBuilderPart to use to build the query</param>
        public QueryBuilder(IEnumerable<IBuilderPart> builderParts)
            : this(builderParts.ToArray())
        {
        }

        private readonly IBuilderPart[] _builderParts;


        /// <summary>
        /// Builds a query from a filter
        /// </summary>
        /// <typeparam name="TEntity">Type of model to filter on</typeparam>
        /// <param name="filter">Filter</param>
        /// <returns>Function to execute filter</returns>
        public Func<IQueryable<TEntity>, IQueryable<TEntity>> BuildQuery<TEntity>(Filter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var pathWalker = new PathWalker<TEntity>(filter);
            pathWalker.WalkPaths();

            if (!pathWalker.IsValid) 
                throw new InvalidPathsException(pathWalker.InvalidPaths);
                          
            var sourceType = Type.GetType($"System.Linq.IQueryable`1[[{typeof(TEntity).AssemblyQualifiedName}]], System.Linq.Expressions");
            ParameterExpression sourceParameter = Expression.Parameter(sourceType, "a");

            var builderContext = new BuilderContext
            {
                CurrentExpression = sourceParameter,
                Filter = filter,
                SourceEntityType = typeof(TEntity),
                PathWalker = pathWalker,
            };

            foreach (var part in _builderParts)
                builderContext.CurrentExpression = part.BuildExpression(builderContext);

            var lamda = Expression.Lambda<Func<IQueryable<TEntity>, IQueryable<TEntity>>>(builderContext.CurrentExpression, sourceParameter);
            return lamda.Compile();
        }

        private static QueryBuilder _defaultBuilder = null;

        /// <summary>
        /// The default Builder
        /// </summary>
        public static QueryBuilder DefaultBuilder
        {
            get
            {
                return _defaultBuilder ??= BuildDefaultBuilder();
            }
        }

        /// <summary>
        /// Builds the Default Builder
        /// </summary>
        /// <returns>QueryBuilder</returns>
        private static QueryBuilder BuildDefaultBuilder()
        {
            var ruleSetExpressionBuilder = new RuleSetExpressionBuilder(
                new SimpleComparisonRuleBuilder(),
                new InRuleBuilder(),
                new LikeRuleBuilder());

            var skipPart = new SkipPart();
            var takePart = new TakePart();
            var wherePart = new WherePart(ruleSetExpressionBuilder);
            var orderByPart = new OrderByPart();

            return new QueryBuilder(
                wherePart,
                new IncludePart(wherePart, orderByPart, skipPart, takePart),
                orderByPart,
                skipPart,
                takePart);
        }

    }
}
