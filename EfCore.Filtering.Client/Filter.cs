using System;
using System.Collections.Generic;
using System.Linq;

namespace EfCore.Filtering.Client
{
    /// <summary>
    /// Filter for a specific entity type
    /// </summary>
    public class Filter<TEntity> : Filter
        where TEntity : class
    {

    }

    /// <summary>
    /// Filter 
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// number of records to take
        /// </summary>
        public int? Take { get; set; }
        
        /// <summary>
        /// number of records to skip
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// A rule set defining the where clause
        /// </summary>
        public RuleSet WhereClause { get; set; }

        /// <summary>
        /// A list of order by
        /// </summary>
        public List<OrderBy> Ordering { get; set; } = new ();

        private List<Include> _includes = new();

        /// <summary>
        /// List of includes
        /// </summary>
        public List<Include> Includes
        {
            get { return _includes; }
            set
            {
                var duplicatePaths = value.GroupBy(x => x.Path)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key);

                if (duplicatePaths.Any())
                    throw new ArgumentException("Duplicate paths supplied, only unique paths allows", nameof(value));
                
                _includes = value;
            }
        }

        /// <summary>
        /// determines if the filter has a where clause
        /// </summary>
        public bool HasWhereClauseRules
        {
            get
            {
                return WhereClause != null && WhereClause.HasRulesOrRuleSets();
            }
        }

        /// <summary>
        /// determines if the filter is populated
        /// </summary>
        public bool IsPopulated
        {
            get
            {
                return Skip.HasValue ||
                       Take.HasValue ||
                       HasWhereClauseRules ||
                       (Ordering != null && Ordering.Count > 0) ||
                       (Includes != null && Includes.Count > 0); 
            }
        }

        /// <summary>
        /// determines if the filter has includes
        /// </summary>
        public bool HasIncludes
        {
            get
            {
                return Includes != null && Includes.Count() > 0;
            }
        }
    }
}
