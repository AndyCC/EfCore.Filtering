using EfCore.Filtering.Client.Serialization.Common;
using System;
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

            if (reader.TokenType == JsonTokenType.StartObject)
                return Reader.Read(ref reader, options, new ReaderOptions<OrderBy>());

            throw new JsonException("OrderBy - No string or object");
        }       

        public override void Write(Utf8JsonWriter writer, OrderBy value, JsonSerializerOptions options)
        {
            var orderPart = value.Order == Ordering.DESC ? "-" : "+";
            var stringValue = $"{orderPart}{value.Path}";
            writer.WriteStringValue(stringValue);
        }
    }
}
