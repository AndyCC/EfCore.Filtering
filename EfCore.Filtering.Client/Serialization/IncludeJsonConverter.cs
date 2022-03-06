using EfCore.Filtering.Client.Serialization.Common;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public class IncludeJsonConverter : JsonConverter<Include>
    {
        static IncludeJsonConverter()
        {
            var propertyMap = new Dictionary<string, string>  
            {
                { "P", nameof(Include.Path) },
                { "F", nameof(Include.Filter) }
            };

            ReaderOptions = new ReaderOptions<Include>(propertyMap);
        }

        private static readonly ReaderOptions<Include> ReaderOptions;

        public override Include Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.None)
                reader.Read();

            if(reader.TokenType == JsonTokenType.StartObject)
                return Reader.Read(ref reader, options, ReaderOptions);

            throw new JsonException("Include - No Object");
        }

        public override void Write(Utf8JsonWriter writer, Include value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
