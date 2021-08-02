using System;
using System.Linq.Expressions;

namespace EfCore.Filtering.Client
{
    /// <summary>
    /// An ordering clause
    /// </summary>
    public class OrderBy
    {
        /// <summary>
        /// Object path from source entity to reach the property to order by
        /// </summary>
        public string Path { get; set; }

        private string _order;
        /// <summary>
        /// Ordering, ASC, DESC, Ascending, Descending - case insensitive
        /// </summary>
        public string Order
        {
            get { return _order; }
            set
            {
                var normalisedValue = value.ToLower();

                _order = value.ToLower() switch
                {
                    "asc" or "ascending" or "desc" or "descending" => normalisedValue,
                    _ => throw new NotSupportedException($"Can not order {value}"),
                };
            }
        }

        /// <summary>
        /// Converst a string to an order by 
        /// "Id" or "+Id" will create an Order By "Id" ascending
        /// "-Id" will create an Order By "Id" descending
        /// </summary>
        /// <param name="order">short string such as "PropertyPath", "+PropertyPath" or "-PropertyPath"</param>
        /// <returns>OrderBy</returns>
        internal static OrderBy FromString(string order)
        {
            if (string.IsNullOrWhiteSpace(order))
                throw new ArgumentException("must have a value", nameof(order));

            string direction = Ordering.ASC;
            string path = order;
            var orderValue = order[0];

            if (orderValue == '+')
                path = order[1..];
            else if (orderValue == '-')
            {
                direction = Ordering.DESC;
                path = order[1..];
            }

            return new OrderBy
            {
                Order = direction,
                Path = path
            };
        }
    }

    /// <summary>
    /// An ordering clause
    /// </summary>
    /// <typeparam name="TSource">source type to apply rule to</typeparam>
    public class OrderBy<TSource> : OrderBy
    {
        /// <summary>
        /// Epression to set the path
        /// </summary>
        public Expression<Func<TSource, object>> PathExpression
        {
            set
            {
                var pathParts = PropertyPath.GetPathParts<TSource, object>(value);
                Path = string.Join(".", pathParts);
            }
        }
    }
}
