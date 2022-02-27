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
        public const string Like = "like";
        
        /// <summary>
        /// In Operator
        /// </summary>
        public const string In = "in"; 
        
        /// <summary>
        /// Equal operator == 
        /// </summary>
        public const string Equal = "eq"; 
        
        /// <summary>
        /// Not equal operator != 
        /// </summary>
        public const string NotEqual = "ne"; 
        
        /// <summary>
        /// Greater Than operator > 
        /// </summary>
        public const string GreaterThan = "gt"; 
        
        /// <summary>
        /// Greater Than or Equal operator >= 
        /// </summary>
        public const string GreaterThanOrEqual =  "gte";

        /// <summary>
        /// Less Than Operator
        /// </summary>
        public const string LessThan = "lt";

        /// <summary>
        /// Less Than Or equal operator <= 
        /// </summary>
        public const string LessThanOrEqual = "lte";
    }
}
