using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public class OrderByJsonConverter : JsonConverter<OrderBy>
    {
        public override OrderBy Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.None)
                reader.Read();

            if(reader.TokenType == JsonTokenType.String)
                return OrderBy.FromString(reader.GetString());

            if(reader.TokenType == JsonTokenType.StartObject)
                return ReadOrderByObject(ref reader, typeToConvert, options);

            throw new JsonException("OrderBy - No string or object");
        }       
        
        private static OrderBy ReadOrderByObject(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var orderBy = new OrderBy();
            PropertyInfo currentProperyInfo = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return orderBy;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var actualPropertyName = reader.GetString();
                    currentProperyInfo = typeToConvert.GetProperty(actualPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                }
                else if (reader.TokenType == JsonTokenType.String)
                    currentProperyInfo.SetValue(orderBy, reader.GetString());

            }

            throw new JsonException("OrderBy - No end of object");
        }

        public override void Write(Utf8JsonWriter writer, OrderBy value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
