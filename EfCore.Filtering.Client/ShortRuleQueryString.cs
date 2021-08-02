namespace EfCore.Filtering.Client
{
    /// <summary>
    /// A Rule with shortened property names for use in a query string
    /// </summary>
    public class ShortRuleQueryString
    {
        /// <summary>
        /// Path
        /// </summary>
        public string P { get; set; }

        /// <summary>
        /// Comparison Operator
        /// </summary>
        public string C { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string V { get; set; }

        /// <summary>
        /// Creates a Rule
        /// </summary>
        /// <returns>Rule</returns>
        public Rule ToRule()
        {
            return new Rule
            {
                ComparisonOperator = C,
                Path = P,
                Value = V
            };
        }
    }
}
