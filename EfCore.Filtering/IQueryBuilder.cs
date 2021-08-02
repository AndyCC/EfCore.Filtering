using EfCore.Filtering.Client;
using System;
using System.Linq;

namespace EfCore.Filtering
{
    /// <summary>
    /// Interface to the query builder
    /// </summary>
    public interface IQueryBuilder
    {
        /// <summary>
        /// Builds a query from a filter
        /// </summary>
        /// <typeparam name="TEntity">Type of model to filter on</typeparam>
        /// <param name="filter">Filter</param>
        /// <returns>Function to execute filter</returns>
        Func<IQueryable<TEntity>, IQueryable<TEntity>> BuildQuery<TEntity>(Filter filter);
    }
}
