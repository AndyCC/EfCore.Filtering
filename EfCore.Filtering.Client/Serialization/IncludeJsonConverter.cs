using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public class IncludeJsonConverter : JsonConverter<Include>
    {
        public override Include Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.None)
                reader.Read();

            if(reader.TokenType == JsonTokenType.StartObject)
                return ReadInclude(ref reader, typeToConvert, options);

            throw new JsonException("Include - No Object");
        }

        private static Include ReadInclude(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var include = new Include();
            PropertyInfo currentPropertyInfo = null;

            while(reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return include;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var actualPropertyName = ShortFormPropertyNameMap.GetLongFormPropertyName<Include>(reader.GetString());
                    currentPropertyInfo = typeToConvert.GetProperty(actualPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                }
                else
                {
                    if (reader.TokenType == JsonTokenType.String)
                        currentPropertyInfo.SetValue(include, reader.GetString());
                    else if(currentPropertyInfo.Name == nameof(Include.Filter))
                        include.Filter = JsonSerializer.Deserialize<Filter>(ref reader, options);
                }
            }

            throw new JsonException("Include - No End Of Object");
        }
        public override void Write(Utf8JsonWriter writer, Include value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
