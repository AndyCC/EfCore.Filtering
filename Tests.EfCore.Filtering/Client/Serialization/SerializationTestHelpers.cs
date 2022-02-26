using EfCore.Filtering.Client.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    internal static class SerializationTestHelpers
    {
        public static Utf8JsonReader GetJsonReader(this string json)
        {
            return new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        }

        public static JsonSerializerOptions SerializeOptions
        {
            get
            {
                var options = new JsonSerializerOptions();
                options.AddFilterConvertors();
                return options;
            }
        }

        public static IEnumerable<object> RuleComparisonOperators
        {
            get
            {
                yield return "in";
                yield return "like";
                yield return "eq";
                yield return "equals";
                yield return "ne";
                yield return "notEqual";
                yield return "ge";
                yield return "greaterThan";
                yield return "gte";
                yield return "greaterThanOrEqual";
                yield return "lt";
                yield return "lessThan";
                yield return "lte";
                yield return "lessThanOrEqual";
            }
        }
    }
}
