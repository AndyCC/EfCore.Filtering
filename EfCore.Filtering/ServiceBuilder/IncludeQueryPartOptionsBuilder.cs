using EfCore.Filtering.Parts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace EfCore.Filtering.ServiceBuilder
{
    /// <summary>
    /// OptionsBuilder for the default Include part
    /// </summary>
    public class IncludeQueryPartOptionsBuilder
    {
        private readonly List<Type> _builderParts = new();

        /// <summary>
        /// Adds a part to use to translate a Include filter
        /// </summary>
        /// <param name="builderPartType">Type that implements IIncludeFilteringPart</param>
        /// <returns>IncludeQueryPartOptionsBuilder</returns>
        public IncludeQueryPartOptionsBuilder UsePart(Type builderPartType)
        {
            if (!builderPartType.IsAssignableTo(typeof(IIncludeFilteringPart)))
                throw new ArgumentException("Must implement IIncludeFilteringPart", nameof(builderPartType));

            if (_builderParts.Contains(builderPartType))
                throw new ArgumentException($"type {builderPartType.FullName} has already been added", nameof(builderPartType));

            _builderParts.Add(builderPartType);
            return this;
        }

        /// <summary>
        /// Tells the Include to use the default Skip part
        /// </summary>
        /// <returns>IncludeQueryPartOptionsBuilder</returns>
        public IncludeQueryPartOptionsBuilder UseDefaultSkip()
        {
            _builderParts.Add(typeof(SkipPart));
            return this;
        }

        /// <summary>
        /// Tells the Include to use the default Take part
        /// </summary>
        /// <returns>IncludeQueryPartOptionsBuilder</returns>
        public IncludeQueryPartOptionsBuilder UseDefaultTake()
        {
            _builderParts.Add(typeof(TakePart));
            return this;
        }

        /// <summary>
        /// Tells the Include to use the default Where part
        /// </summary>
        /// <returns>IncludeQueryPartOptionsBuilder</returns>
        public IncludeQueryPartOptionsBuilder UseDefaultWherePart()
        {
            _builderParts.Add(typeof(WherePart));
            return this;
        }

        /// <summary>
        /// Tells the Include to use the default Order By part
        /// </summary>
        /// <returns>IncludeQueryPartOptionsBuilder</returns>
        public IncludeQueryPartOptionsBuilder UseDefaultOrderBy()
        {
            _builderParts.Add(typeof(OrderByPart));
            return this;
        }

        /// <summary>
        /// Tells the Include to use the default Skip, Take, Where and Order By parts
        /// </summary>
        /// <returns>IncludeQueryPartOptionsBuilder</returns>
        public IncludeQueryPartOptionsBuilder UseDefault()
        {
           return UseDefaultSkip()
            .UseDefaultTake()
            .UseDefaultWherePart()
            .UseDefaultOrderBy();
        }


        /// <summary>
        /// Setup method adds the relevant configuration to the service collection 
        /// </summary>
        /// <param name="serviceCollection">IServiceCollection</param>
        /// <param name="lifetime">lifetime of the services added to the service collection</param>
        internal void Setup(IServiceCollection serviceCollection, ServiceLifetime lifetime)
        {
            foreach (var partType in _builderParts)
                serviceCollection.Add(new ServiceDescriptor(typeof(IIncludeFilteringPart), partType, lifetime));
        }
    }
}
