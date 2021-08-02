using EfCore.Filtering.Client;

namespace EfCore.Filtering
{
    /// <summary>
    /// Extensions for the Filter 
    /// </summary>
    internal static class FilterExtensions
    {
        /// <summary>
        /// Determines if the filter can be used as a filter on an include
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>true if can use as include filter, otherwise false</returns>
        public static bool CanApplyAsIncludeFilter(this Filter filter)
        {
            return filter != null &&
                (filter.Take.HasValue ||
                filter.Skip.HasValue ||
                (filter.Ordering != null && filter.Ordering.Count > 0) ||
                filter.HasWhereClauseRules);
        }               
    }
}
