namespace EfCore.Filtering.Client
{
    /// <summary>
    /// Include with shortened property names
    /// </summary>
    public class ShortInclude
    {
        /// <summary>
        /// Path
        /// </summary>
        public string P { get; set; }

        /// <summary>
        /// Filter
        /// </summary>
        public ShortFilter F { get; set; }

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
