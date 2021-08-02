namespace EfCore.Filtering.Client
{
    /// <summary>
    /// Allowed ordering on the order by 
    /// </summary>
    public static class Ordering
    {
        /// <summary>
        /// Ascending
        /// </summary>
        public static string ASC { get { return "asc"; } }
        /// <summary>
        /// Descending
        /// </summary>
        public static string DESC { get { return "desc"; } }
    }
}
