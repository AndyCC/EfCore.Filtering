using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCore.Filtering.Paths
{
    /// <summary>
    /// Helpers for property paths
    /// </summary>
    internal static class PropertyPath
    {
        internal static string PathSeperator = ".";

        /// <summary>
        /// Converts a property path to an expression which will retrieve the property
        /// </summary>
        /// <param name="fullPath">Property path as a string, seperated by '.' e.g. Property1.Property2 </param>
        /// <param name="existingExpression">expression to add property path to, often a ParameterExpression</param>
        /// <returns>Expression</returns>
        public static Expression AsPropertyExpression(string fullPath, Expression existingExpression)
        {
            return AsPropertyExpression(fullPath.Split(PathSeperator), existingExpression);
        }

        /// <summary>
        ///  Converts a property path to an expression which will retrieve the property
        /// </summary>
        /// <param name="parts">array of string property names in the path to the property that is to be retrieved</param>
        /// <param name="existingExpression">expression to add property path to, often a ParameterExpression</param>
        /// <returns>Expression</returns>
        public static Expression AsPropertyExpression(string[] parts, Expression existingExpression)
        {
            Expression expression = existingExpression;

            foreach (var part in parts)
                expression = Expression.PropertyOrField(expression, part);

            return expression;
        }
    }
}
