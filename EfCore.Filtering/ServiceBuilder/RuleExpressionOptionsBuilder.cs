using EfCore.Filtering.RuleSets;
using EfCore.Filtering.RuleSets.Rules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace EfCore.Filtering.ServiceBuilder
{
    /// <summary>
    /// Options Builder for Rule Expressions
    /// </summary>
    public class RuleExpressionOptionsBuilder
    {
        private readonly List<Type> _builderTypes = new();

        /// <summary>
        /// Specifies a RuleExpressionBuilder should be used
        /// </summary>
        /// <param name="builderType">Type that implements IRuleExpressionBuilder</param>
        /// <returns>RuleExpressionOptionsBuilder</returns>
        public RuleExpressionOptionsBuilder UseRuleExpressionBuilder(Type builderType)
        {
            if (!builderType.IsAssignableTo(typeof(IRuleExpressionBuilder)))
                throw new ArgumentException("Must implement IRuleExpressionBuilder", nameof(builderType));

            if (_builderTypes.Contains(builderType))
                throw new ArgumentException($"type {builderType.FullName} has already been added", nameof(builderType));

            _builderTypes.Add(builderType);

            return this;
        }

        /// <summary>
        /// The default setup, adds the following RuleExpressionBuilders to the list of builders used:
        /// InRuleBuilder
        /// LikeRuleBuilder
        /// SimpleComparisonRuleBuilder
        /// </summary>
        /// <returns>RuleExpressionOptionsBuilder</returns>
        public RuleExpressionOptionsBuilder UseDefault()
        {
            _builderTypes.Add(typeof(InRuleBuilder));
            _builderTypes.Add(typeof(LikeRuleBuilder));
            _builderTypes.Add(typeof(SimpleComparisonRuleBuilder));

            return this;
        }

        /// <summary>
        /// Setup method adds the relevant configuration to the service collection 
        /// </summary>
        /// <param name="serviceCollection">IServiceCollection</param>
        /// <param name="lifetime">lifetime of the services added to the service collection</param>
        internal void Setup(IServiceCollection serviceCollection, ServiceLifetime lifetime)
        {
            foreach (var builderType in _builderTypes)
                serviceCollection.Add(new ServiceDescriptor(typeof(IRuleExpressionBuilder), builderType, lifetime));
        }
    }
}
