using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCore.Filtering.Client
{
    /// <summary>
    /// Provides helper functions to navigate code paths
    /// </summary>
    internal static class PropertyPath
    {
        /// <summary>
        /// Converts an expression path into an array of string names
        /// </summary>
        /// <typeparam name="TSource">Source Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        /// <param name="expression">expression to convert to strings</param>
        /// <returns>array of strings</returns>
        internal static IEnumerable<string> GetPathParts<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            var visitor = new PropertyVisitor();
            visitor.Visit(expression);
            return visitor.Path;
        }

        /// <summary>
        /// Expression Visitor that converts an expression to a list of property names
        /// </summary>
        internal class PropertyVisitor : ExpressionVisitor
        {
            internal PropertyVisitor()
            {
            }

            private readonly List<string> _path = new();
            public IEnumerable<string> Path { get { return _path.AsReadOnly(); } }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (!(node.Member is PropertyInfo))
                {
                    throw new ArgumentException("The path can only contain properties", nameof(node));
                }

                _path.Insert(0, node.Member.Name);
                return base.VisitMember(node);
            }
        }
    }
}
