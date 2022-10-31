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
            var propertyMap = new PropertyMap(new Dictionary<string, string>  
            {
                { "P", nameof(Include.Path) },
                { "F", nameof(Include.Filter) }
            });

            ReaderOptions = new ReaderOptions<Include>(propertyMap.ShortNameToLongName);
            WriterOptions = new WriterOptions<Include>(propertyMap.LongNameToShortName);
        }

        private static readonly ReaderOptions<Include> ReaderOptions;
        private static readonly WriterOptions<Include> WriterOptions;

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
            Writer.Write(writer, options, WriterOptions, value);
        }
    }
}
