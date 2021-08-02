using EfCore.Filtering.Parts;
using EfCore.Filtering.RuleSets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace EfCore.Filtering.ServiceBuilder
{
    /// <summary>
    /// OptionsBuilder for a QueryBuilder
    /// </summary>
    public class QueryBuilderOptionsBuilder
    {
        private readonly List<Type> _builderParts = new();
        private readonly IncludeQueryPartOptionsBuilder _queryPartOptionsBuilder = new();
        private Type _ruleSetExpressionBuilderType = null;
        private readonly RuleExpressionOptionsBuilder _ruleExpressionOptionsBuilder = new();
        private bool _useDefaultInstance = false;

        /// <summary>
        /// Specify a part the query builder is to use
        /// </summary>
        /// <param name="builderPartType">Type that derives from IBuilderPart</param>
        /// <returns>QueryBuilderOptionsBuilder</returns>
        public QueryBuilderOptionsBuilder UsePart(Type builderPartType)
        {
            if (!builderPartType.IsAssignableTo(typeof(IBuilderPart)))
                throw new ArgumentException("Must implement IBuilderPart", nameof(builderPartType));

            if (_builderParts.Contains(builderPartType))
                throw new ArgumentException($"type {builderPartType.FullName} has already been added", nameof(builderPartType));

            _builderParts.Add(builderPartType);

            return this;
        }

        /// <summary>
        /// Specifies the QueryBuilder should use the default Skip part
        /// </summary>
        /// <returns>QueryBuilderOptionsBuilder</returns>
        public QueryBuilderOptionsBuilder UseDefaultSkip()
        {
            _builderParts.Add(typeof(SkipPart));

            return this;
        }

        /// <summary>
        /// Specifies the QueryBuilder should use the default Take part
        /// </summary>
        /// <returns>QueryBuilderOptionsBuilder</returns>
        public QueryBuilderOptionsBuilder UseDefaultTake()
        {
            _builderParts.Add(typeof(TakePart));

            return this;
        }

        /// <summary>
        /// Specifies the QueryBuilder should use the default Order By part
        /// </summary>
        /// <returns>QueryBuilderOptionsBuilder</returns>
        public QueryBuilderOptionsBuilder UseDefaultOrderBy()
        {
            _builderParts.Add(typeof(OrderByPart));

            return this;
        }

        /// <summary>
        /// Specifies the QueryBuilder should use the Default Include part
        /// </summary>
        /// <param name="options">IncludeQueryPartOptionsBuilder, if null uses the Default setup</param>
        /// <returns>QueryBuilderOptionsBuilder</returns>
        public QueryBuilderOptionsBuilder UseDefaultIncludePart(Action<IncludeQueryPartOptionsBuilder> options = null)
        {
            _builderParts.Add(typeof(IncludePart));

            if (options == null)
                _queryPartOptionsBuilder.UseDefault();
            else
                options(_queryPartOptionsBuilder);

            return this;
        }

        /// <summary>
        /// Specifies the QueryBuilder should use the default Where part
        /// </summary>
        /// <returns>QueryBuilderOptionsBuilder</returns>
        public QueryBuilderOptionsBuilder UseDefaultWherePart()
        {
            _builderParts.Add(typeof(WherePart));

            return this;
        }

        /// <summary>
        /// Specifies the RuleSetExpressionBuilder to use
        /// </summary>
        /// <param name="builderType">Type that implements IRuleSetExpressionBuilder</param>
        /// <returns>QueryBuilderOptionsBuilder</returns>
        public QueryBuilderOptionsBuilder UseRuleSetExpressionBuilder(Type builderType)
        {
            if (!builderType.IsAssignableTo(typeof(IRuleSetExpressionBuilder)))
                throw new ArgumentException("Must implement IRuleSetExpressionBuilder", nameof(builderType));

            _ruleSetExpressionBuilderType = builderType;

            return this;
        }

        /// <summary>
        /// Specifies the RuleSetExpressionBuilder used should be RuleSetExpressionBuilder
        /// </summary>
        /// <returns>QueryBuilderOptionsBuilder</returns>
        public QueryBuilderOptionsBuilder UseDefaulRuleSetExpressionBuilder()
        {
            _ruleSetExpressionBuilderType = typeof(RuleSetExpressionBuilder);

            return this;
        }

        /// <summary>
        /// Specifies the RuleSetExpressionBuilders to use
        /// </summary>
        /// <param name="options">RuleExpressionOptionsBuilder, if null and the RuleSetExpressionBuilderType is the default then uses the default setup for the rules</param>
        /// <returns>QueryBuilderOptionsBuilder</returns>
        public QueryBuilderOptionsBuilder UseRuleBuilders(Action<RuleExpressionOptionsBuilder> options = null)
        {
            if (options == null && _ruleSetExpressionBuilderType == typeof(RuleSetExpressionBuilder))
                _ruleExpressionOptionsBuilder.UseDefault();
            else 
                options?.Invoke(_ruleExpressionOptionsBuilder);

            return this;
        }

        /// <summary>
        /// Default setup for the QueryBuilder
        /// </summary>
        /// <param name="includeOptions">IncludeQueryPartOptionsBuilder, if null uses the default setup</param>
        /// <param name="ruleExpressionOptions">RuleExpressionOptionsBuilder, if null uses the default setup</param>
        /// <returns><QueryBuilderOptionsBuilder/returns>
        public QueryBuilderOptionsBuilder UseDefault(Action<IncludeQueryPartOptionsBuilder> includeOptions = null, Action<RuleExpressionOptionsBuilder> ruleExpressionOptions = null)
        {
            return UseDefaultSkip()
              .UseDefaultTake()
              .UseDefaultOrderBy()
              .UseDefaultIncludePart(includeOptions)
              .UseDefaultWherePart()
              .UseDefaulRuleSetExpressionBuilder()
              .UseRuleBuilders(ruleExpressionOptions);
        }

        /// <summary>
        /// Set the Service Collection to use the default setup as an instance. 
        /// So an instance will be created and added to the service collection
        /// </summary>
        public void UseDefaultInstance()
        {
            _useDefaultInstance = true;
        }

        /// <summary>
        /// Setup method adds the relevant configuration to the service collection 
        /// </summary>
        /// <param name="serviceCollection">IServiceCollection</param>
        /// <param name="lifetime">lifetime of the services added to the service collection</param>
        internal void Setup(IServiceCollection serviceCollection, ServiceLifetime lifetime)
        {
            if(_useDefaultInstance)
            {
                if (lifetime != ServiceLifetime.Singleton)
                    throw new InvalidOperationException("Can not use default instance of QueryBuilder when the lifetime is not singleton");

                serviceCollection.AddSingleton(typeof(IQueryBuilder), QueryBuilder.DefaultBuilder);
                return;
            }

            foreach (var builderPartType in _builderParts)
                serviceCollection.Add(new ServiceDescriptor(typeof(IBuilderPart), builderPartType, lifetime));

            _queryPartOptionsBuilder.Setup(serviceCollection, lifetime);

            if (_ruleSetExpressionBuilderType != null)
                serviceCollection.Add(new ServiceDescriptor(typeof(IRuleSetExpressionBuilder), _ruleSetExpressionBuilderType, lifetime));

            _ruleExpressionOptionsBuilder.Setup(serviceCollection, lifetime);

            serviceCollection.Add(new ServiceDescriptor(typeof(IQueryBuilder), typeof(QueryBuilder), lifetime));
        }
    }
}
