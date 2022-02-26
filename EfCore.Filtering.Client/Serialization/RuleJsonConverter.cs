using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public class RuleJsonConverter : JsonConverter<Rule>
    {
        public override Rule Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.None)
                reader.Read();

            if (reader.TokenType == JsonTokenType.StartObject)
                return RuleJsonConverter.ReadRule(ref reader, typeToConvert, options);
            
            throw new JsonException("Rule - No Object");
        }

        private static Rule ReadRule(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var rule = new Rule();
            PropertyInfo currentProperyInfo = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return rule;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var actualPropertyName = ShortFormPropertyNameMap.GetLongFormPropertyName<Rule>(reader.GetString());
                    currentProperyInfo = typeToConvert.GetProperty(actualPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                }
                else
                {
                    if (currentProperyInfo.Name == nameof(Rule.Value))
                        rule.Value = JsonSerializer.Deserialize<object>(ref reader, options);
                    else if (reader.TokenType == JsonTokenType.String)
                        currentProperyInfo.SetValue(rule, reader.GetString());
                }
            }

            throw new JsonException("Rule - No End Of Object");
        }

        public override void Write(Utf8JsonWriter writer, Rule value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
