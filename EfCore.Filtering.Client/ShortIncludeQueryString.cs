namespace EfCore.Filtering.Client
{
    /// <summary>
    /// Include with shortened property names for use in a QueryString
    /// </summary>
    public class ShortIncludeQueryString
    {
        /// <summary>
        /// Path
        /// </summary>
        public string P { get; set; }

        /// <summary>
        /// Filter
        /// </summary>
        public ShortFilterQueryString F { get; set; }

        /// <summary>
        /// Creates an Include
        /// </summary>
        /// <returns>Include</returns>
        public Include ToInclude()
        {
            return new Include
            {
                Path = P,
                Filter = F?.ToFilter()
            };
        }
    }
}
