using EfCore.Filtering.Client.Serialization.Common;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public class RuleSetJsonConverter : JsonConverter<RuleSet>
    {
        static RuleSetJsonConverter()
        {
             var propertyMap = new PropertyMap(new Dictionary<string, string>
                {
                    { "R", nameof(RuleSet.Rules) },
                    { "L", nameof(RuleSet.LogicalOperator) },
                    { "S", nameof(RuleSet.RuleSets) },
                });

            var readPropertyTranslators = new Dictionary<string, Func<string, string>>
            {
                { nameof(RuleSet.LogicalOperator),  TranslateLogicalOperatorForRead}
            };

            ReaderOptions = new ReaderOptions<RuleSet>(propertyMap.ShortNameToLongName, stringPropertyTranslators: readPropertyTranslators);

            var writePropertyTranslators = new Dictionary<string, Func<string, string>>
            {
                { nameof(RuleSet.LogicalOperator),  TranslateLogicalOperatorForWrite}
            };

            WriterOptions = new WriterOptions<RuleSet>(propertyMap.LongNameToShortName, stringPropertyTranslators: writePropertyTranslators);
        }

        private static readonly ReaderOptions<RuleSet> ReaderOptions;
        private static readonly WriterOptions<RuleSet> WriterOptions;

        public override RuleSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.None)
                reader.Read();

            if (reader.TokenType == JsonTokenType.StartObject)
                return Reader.Read(ref reader, options, ReaderOptions);

            throw new JsonException("RuleSet - No Object");
        }

        private static string TranslateLogicalOperatorForRead(string input)
        {
            return (input.ToUpper()) switch
            {
                "A" or LogicalOperators.AND => LogicalOperators.AND,
                "O" or LogicalOperators.OR => LogicalOperators.OR,
                _ => throw new JsonException($"Unsuported Logical Operator {input}")
            };
        }

        private static string TranslateLogicalOperatorForWrite(string input)
        {
            return (input.ToUpper()) switch
            {
                "A" or LogicalOperators.AND => "A",
                "O" or LogicalOperators.OR => "O",
                _ => throw new JsonException($"Unsuported Logical Operator {input}")
            };
        }

        public override void Write(Utf8JsonWriter writer, RuleSet value, JsonSerializerOptions options)
        {
            Writer.Write(writer, options, WriterOptions, value);
        }
    }
}
