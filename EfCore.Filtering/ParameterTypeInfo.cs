using System;

namespace EfCore.Filtering
{
    /// <summary>
    /// defines a parameter on a method
    /// </summary>
    /// <remarks>for use with TypeExtensions.GetGenericMethod</remarks>
    public class ParameterTypeInfo
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rootType">Root Type (e.g. Expression<>)</param>
        /// <param name="innerTypes">any inner types (e.g. Func<> or null if unknown)</param>
        public ParameterTypeInfo(Type rootType, params ParameterTypeInfo[] innerTypes)
        {
            RootType = rootType;
            InnerTypes = innerTypes;
        }

        /// <summary>
        /// Root Type (e.g. Expression<>)
        /// </summary>
        public Type RootType {get; private set;}

        /// <summary>
        /// any inner types (e.g. Func<> or null if unknown)
        /// </summary>
        public ParameterTypeInfo[] InnerTypes { get; private set; }
    }
}
