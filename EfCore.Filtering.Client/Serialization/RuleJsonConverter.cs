using EfCore.Filtering.Client.Serialization.Common;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public class RuleJsonConverter : JsonConverter<Rule>
    {
        static RuleJsonConverter()
        {
            var propertyMap = new Dictionary<string, string>
            {
                { "P", nameof(Rule.Path) },
                { "C", nameof(Rule.ComparisonOperator) },
                { "V", nameof(Rule.Value) },
            };

            var propertyReadInterceptors = new Dictionary<string, InterceptPropertyRead<Rule>>
            {
                {nameof(Rule.Value), SetValueFromObject}
            };

            ReaderOptions = new ReaderOptions<Rule>(propertyMap, propertyReadInterceptors);
        }

        private static readonly ReaderOptions<Rule> ReaderOptions;
        public override Rule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.None)
                reader.Read();

            if (reader.TokenType == JsonTokenType.StartObject)
                return Reader.Read<Rule>(ref reader, options, ReaderOptions);
            
            throw new JsonException("Rule - No Object");
        }

        private static void SetValueFromObject(Rule rule, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            rule.Value = JsonSerializer.Deserialize<object>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, Rule value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
