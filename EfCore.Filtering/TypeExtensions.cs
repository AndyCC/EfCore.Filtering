using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace EfCore.Filtering
{
    /// <summary>
    /// Extensions for a type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the underlying type if the Type supplied implements IEnumerable and is generic
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Type - first generic argument. Or null if the supplied type does not implement IEnumerable or is not generic</returns>
        public static Type GetUnderlyingTypeIfGenericAndEnumerable(this Type type)
        {
            if (!type.IsAssignableTo(typeof(IEnumerable)))
                return null;

            if (!type.IsGenericType)
                return null;

            return type.GetGenericArguments()[0];
        }

        /// <summary>
        /// Gets the MethodInfo for a generic method
        /// </summary>
        /// <param name="type">Type the method is on</param>
        /// <param name="name">name of the method</param>
        /// <param name="bindingFlags">binding flags</param>
        /// <param name="parameterTypes">Parameters the method must have</param>
        /// <returns>MethodInfo</returns>
        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags bindingFlags, params ParameterTypeInfo[] parameterTypes)
        {
            var methods = type.GetMethods(bindingFlags).Where(x => x.Name == name);

            foreach(var method in methods)
            {
               var parameters = method.GetParameters();

                if (parameters.Length != parameterTypes.Length)
                    continue;

                var allTypesMatch = true; 

                for(var i = 0; i < parameters.Length; i++)
                {
                    if(!IsTypeOrGenericTypeOf(parameters[i].ParameterType, parameterTypes[i]))
                    {
                        allTypesMatch = false;
                        break;
                    }    
                }

                if (allTypesMatch)
                    return method;
            }

            return null;
        }

        /// <summary>
        /// Checks if the supplied type is either the type desiredType or the supplied type is generic and it's generic type definition matches the desiredType
        /// </summary>
        /// <param name="type">type to check</param>
        /// <param name="desiredType">ParameterTypeInfo representing the type required</param>
        /// <returns>true if matches, otherwise false</returns>
        private static bool IsTypeOrGenericTypeOf(Type type, ParameterTypeInfo desiredType)
        {
            if (!type.IsGenericType)
            {
                if (type.FullName == null)
                    return desiredType.RootType == null || desiredType.RootType == typeof(object);

                return type == desiredType.RootType;
            }

            var genericType = type.GetGenericTypeDefinition();

            if (genericType != desiredType.RootType)
                return false;

            if (desiredType.InnerTypes.Length > 0)
            {
                var genericTypeArguments = type.GetGenericArguments();

                if (genericTypeArguments.Length != desiredType.InnerTypes.Length)
                    return false;

                for (var i = 0; i < genericTypeArguments.Length; i++)
                {
                    var genericArgumentsMatch = IsTypeOrGenericTypeOf(genericTypeArguments[i], desiredType.InnerTypes[i]);

                    if (!genericArgumentsMatch)
                        return false;

                }
            }

            return true;
        }
    }
}
