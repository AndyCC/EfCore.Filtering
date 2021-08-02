namespace EfCore.Filtering.Client
{
    /// <summary>
    /// List of default rule operators
    /// </summary>
    public static class ComparisonOperators
    {
        /// <summary>
        /// SQL Like operator
        /// </summary>
        public static string Like
        {
            get { return "like"; }
        }

        /// <summary>
        /// In Operator
        /// </summary>
        public static string In
        {
            get { return "in"; }
        }

        /// <summary>
        /// Equal operator == 
        /// </summary>
        public static string Equal
        {
            get { return "eq"; }
        }

        /// <summary>
        /// Not equal operator != 
        /// </summary>
        public static string NotEqual
        {
            get { return "ne"; }
        }

        /// <summary>
        /// Greater Than operator > 
        /// </summary>
        public static string GreaterThan
        {
            get { return "gt"; }
        }

        /// <summary>
        /// Greater Than or Equal operator >= 
        /// </summary>
        public static string GreaterThanOrEqual
        {
            get { return "gte"; }
        }

        /// <summary>
        /// Less Than Operator
        /// </summary>
        public static string LessThan
        {
            get { return "lt"; }
        }

        /// <summary>
        /// Less Than Or equal operator <= 
        /// </summary>
        public static string LessThanOrEqual
        {
            get { return "lte"; }
        }
    }
}
