using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public class FilterJsonConverter : JsonConverter<Filter>
    {
        //TODO: this needs a path walker to complete serialization
        public override Filter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.None)
                reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Filter - Object does not start");

            AddRequiredConvertors(options);

            var filter = new Filter();

            PropertyInfo currentProperyInfo = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return filter;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var actualPropertyName = ShortFormPropertyNameMap.GetLongFormPropertyName<Filter>(reader.GetString());
                    currentProperyInfo = typeToConvert.GetProperty(actualPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                }
                else
                {
                    if (reader.TokenType == JsonTokenType.Number)
                        currentProperyInfo.SetValue(filter, reader.GetInt32());

                    if (reader.TokenType == JsonTokenType.StartArray || reader.TokenType == JsonTokenType.StartObject)
                    {
                        var value = JsonSerializer.Deserialize(ref reader, currentProperyInfo.PropertyType, options);
                        currentProperyInfo.SetValue(filter, value);
                    }
                }
            }

            throw new JsonException("Filter - Object does not end");
        }        

        private static void AddRequiredConvertors(JsonSerializerOptions options)
        {
            AddConvertor(options, new OrderByJsonConverter());
        }

        private static void AddConvertor(JsonSerializerOptions options, OrderByJsonConverter orderBySerializer)
        {
            if (options.Converters.All(x => x.GetType() != orderBySerializer.GetType()))
                options.Converters.Add(orderBySerializer);
        }

        public override void Write(Utf8JsonWriter writer, Filter value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
