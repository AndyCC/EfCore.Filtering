using Microsoft.Extensions.DependencyInjection;
using System;

namespace EfCore.Filtering.ServiceBuilder
{
    /// <summary>
    /// Service Collection Extensions to Add a QueryBuilder
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds an IQueryBuilder to the service collection
        /// </summary>
        /// <param name="serviceCollection">IServiceCollection to add the IQueryBuilder to</param>
        /// <param name="options">QueryBuilderOptionsBuilder, if null used the Default Instance</param>
        /// <param name="lifetime">Lifetime for the QueryBuilder, default to Singleton</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddQueryBuilder(this IServiceCollection serviceCollection, Action<QueryBuilderOptionsBuilder> options = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var optionsBuilder = new QueryBuilderOptionsBuilder();

            if (options == null)
                optionsBuilder.UseDefaultInstance();
            else
                options(optionsBuilder);

            optionsBuilder.Setup(serviceCollection, lifetime);

            return serviceCollection;
        }
    }
}
