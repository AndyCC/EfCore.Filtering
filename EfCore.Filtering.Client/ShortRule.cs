namespace EfCore.Filtering.Client
{
    /// <summary>
    /// A Rule with shortened property names
    /// </summary>
    public class ShortRule
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
        public object V { get; set; }

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
