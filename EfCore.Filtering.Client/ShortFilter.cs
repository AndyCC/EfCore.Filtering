using System;
using System.Linq;

namespace EfCore.Filtering.Client
{
    /// <summary>
    /// A filter with shorter property names
    /// </summary>
    public class ShortFilter
    {
        /// <summary>
        /// Take
        /// </summary>
        public int? T { get; set; }

        /// <summary>
        /// Skip
        /// </summary>
        public int? S { get; set; }

        /// <summary>
        /// Ordering, specified by a shortned string such as "PropertyPath", "+PropertyPath" or "-PropertyPath"
        /// </summary>
        public string[] O { get; set; }

        /// <summary>
        /// Where
        /// </summary>
        public ShortRuleSet W { get; set; }

        /// <summary>
        /// Includes
        /// </summary>
        public ShortInclude[] I { get; set; }

        /// <summary>
        /// Converts type ShortFilter to a Filter 
        /// </summary>
        /// <param name="filterType">type of Filter to create, must derive from Filter</param>
        /// <returns>Filter</returns>
        public Filter ToFilter(Type filterType = null)
        {
            if (filterType == null)
                filterType = typeof(Filter);

            if (!filterType.IsAssignableTo(typeof(Filter)))
                throw new ArgumentException($"must derive from {typeof(Filter).FullName}", nameof(filterType));

            var filter = (Filter)Activator.CreateInstance(filterType);

            filter.Take = T;
            filter.Skip = S;           

            if (O != null)
                filter.Ordering = O.Select(x => OrderBy.FromString(x)).ToList();

            if (W != null)
                filter.WhereClause = W.ToRuleSet();

            if (I != null)
                filter.Includes = I.Select(x => x.ToInclude()).ToList();

            return filter;
        }

        /// <summary>
        /// Determines if the ShortFilter is valid
        /// </summary>
        /// <returns>true if valid, otherwise false</returns>
        public bool IsValid()
        {
            return T != null || S != null || O != null || W != null || I != null;
        }
    }
}
