using System;
using System.Reflection;
using System.Text.Json;

namespace EfCore.Filtering.Client.Serialization.Common
{
    public static class Reader
    {
        public static T Read<T>(ref Utf8JsonReader reader, JsonSerializerOptions jsonOptions, ReaderOptions<T> readOptions, T model = null)
            where T : class
        {
            if (model == null)
                model = Activator.CreateInstance<T>();

            PropertyInfo currentProperyInfo = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return model;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    currentProperyInfo = LocatePropertyInfo(ref reader, readOptions);
                }
                else if (readOptions.PropertyReadInterceptors.ContainsKey(currentProperyInfo.Name))
                {
                    readOptions.PropertyReadInterceptors[currentProperyInfo.Name](model, ref reader, jsonOptions);
                }
                else if (reader.TokenType == JsonTokenType.StartArray || reader.TokenType == JsonTokenType.StartObject)
                {
                    var value = JsonSerializer.Deserialize(ref reader, currentProperyInfo.PropertyType, jsonOptions);
                    currentProperyInfo.SetValue(model, value);
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    string value = ReadStringValue(ref reader, readOptions, currentProperyInfo);
                    currentProperyInfo.SetValue(model, value);
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    SetNumericValue(ref reader, model, currentProperyInfo);
                }
            }

            throw new JsonException($"{typeof(T).Name} - No End Of Object");
        }

        private static PropertyInfo LocatePropertyInfo<T>(ref Utf8JsonReader reader, ReaderOptions<T> readOptions)
        {
            var actualPropertyName = reader.GetString();

            if (actualPropertyName.Length == 1)
            {
                var searchPropertyName = actualPropertyName.ToUpper();

                if (!readOptions.ShortToLongForPropertyNameMap.ContainsKey(searchPropertyName))
                    throw new JsonException($"{searchPropertyName} property can not be mapped on type {typeof(T).FullName}");

                actualPropertyName = readOptions.ShortToLongForPropertyNameMap[searchPropertyName];
            }

            return typeof(T).GetProperty(actualPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        }
        private static string ReadStringValue<T>(ref Utf8JsonReader reader, ReaderOptions<T> readOptions, PropertyInfo currentProperyInfo)
        {
            var value = reader.GetString();

            if (readOptions.StringPropertyTranslators.ContainsKey(currentProperyInfo.Name))
                value = readOptions.StringPropertyTranslators[currentProperyInfo.Name](value);

            return value;
        }

        private static void SetNumericValue<T>(ref Utf8JsonReader reader, T model, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(short) || propertyInfo.PropertyType == typeof(short?))
                propertyInfo.SetValue(model, reader.GetInt16());
            else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                propertyInfo.SetValue(model, reader.GetInt32());
            else if (propertyInfo.PropertyType == typeof(long) || propertyInfo.PropertyType == typeof(long?))
                propertyInfo.SetValue(model, reader.GetInt64());
            else if (propertyInfo.PropertyType == typeof(float) || propertyInfo.PropertyType == typeof(float?))
                propertyInfo.SetValue(model, reader.GetSingle());
            else if (propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(double?))
                propertyInfo.SetValue(model, reader.GetDouble());
            else if (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?))
                propertyInfo.SetValue(model, reader.GetDecimal());
            else
                throw new NotImplementedException($"Can not read numeric type {propertyInfo.PropertyType.FullName}");
        }
    }
}
