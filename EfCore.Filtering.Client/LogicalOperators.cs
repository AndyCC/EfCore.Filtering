using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCore.Filtering.Client
{
    /// <summary>
    /// Logical operators allowed by the ruleset
    /// </summary>
    public static class LogicalOperators
    {
        /// <summary>
        /// AND
        /// </summary>
        public static string AND { get { return "AND"; } }

        /// <summary>
        /// OR
        /// </summary>
        public static string OR { get { return "OR"; } }
    }
}
