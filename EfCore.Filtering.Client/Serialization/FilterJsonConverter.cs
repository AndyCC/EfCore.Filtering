using EfCore.Filtering.Client.Serialization.Common;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public class FilterJsonConverter : JsonConverter<Filter>
    {
        static FilterJsonConverter()
        {
            var propertyMap = new PropertyMap(new Dictionary<string, string>
            {
                { "T", nameof(Filter.Take) },
                { "S", nameof(Filter.Skip) },
                { "O", nameof(Filter.Ordering) },
                { "W", nameof(Filter.WhereClause) },
                { "I", nameof(Filter.Includes) }
            });

            ReaderOptions = new ReaderOptions<Filter>(propertyMap.ShortNameToLongName);
            WriterOptions = new WriterOptions<Filter>(propertyMap.LongNameToShortName);
        }

        private static readonly ReaderOptions<Filter> ReaderOptions;
        private static readonly WriterOptions<Filter> WriterOptions;


        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableTo(typeof(Filter));
        }

        public override Filter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.None)
                reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Filter - Object does not start");

            var filter = (Filter)Activator.CreateInstance(typeToConvert);
            return Reader.Read(ref reader, options, ReaderOptions, filter);

            throw new JsonException("Filter - Object does not end");
        }        

        public override void Write(Utf8JsonWriter writer, Filter value, JsonSerializerOptions options)
        {
            Writer.Write(writer, options, WriterOptions, value);
        }
    }
}
