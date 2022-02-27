using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public class RuleSetJsonConverter : JsonConverter<RuleSet>
    {
        public override RuleSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.None)
                reader.Read();

            if (reader.TokenType == JsonTokenType.StartObject)
                return ReadRuleSet(ref reader, typeToConvert, options);

            throw new JsonException("RuleSet - No Object");
        }

        private static RuleSet ReadRuleSet(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var ruleSet = new RuleSet();
            PropertyInfo currentProperyInfo = null;

            while(reader.Read())
            {
                if(reader.TokenType == JsonTokenType.EndObject)
                    return ruleSet;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var actualPropertyName = ShortFormPropertyNameMap.GetLongFormPropertyName<RuleSet>(reader.GetString());
                    currentProperyInfo = typeToConvert.GetProperty(actualPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                }
                else if (reader.TokenType == JsonTokenType.StartArray || reader.TokenType == JsonTokenType.StartObject)
                {
                    var value = JsonSerializer.Deserialize(ref reader, currentProperyInfo.PropertyType, options);
                    currentProperyInfo.SetValue(ruleSet, value);
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    var value = reader.GetString();

                    if(currentProperyInfo.Name == nameof(RuleSet.LogicalOperator))
                        value = TransalateLogicalOperator(value);

                    currentProperyInfo.SetValue(ruleSet, value);
                }
            }

            throw new JsonException("RuleSet - No End Of Object");
        }

        private static string TransalateLogicalOperator(string input)
        {
            return (input.ToUpper()) switch
            {
                "A" or LogicalOperators.AND => LogicalOperators.AND,
                "O" or LogicalOperators.OR => LogicalOperators.OR,
                _ => throw new JsonException($"Unsuported Logical Operator {input}")
            };
        }

        public override void Write(Utf8JsonWriter writer, RuleSet value, JsonSerializerOptions options)
        {

            throw new NotImplementedException();
        }
    }
}
