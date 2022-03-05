using System.Collections.Generic;
using System.Text.Json;

namespace EfCore.Filtering.Client.Serialization
{
    internal static class ShortFormPropertyNameMap
    {
        private static readonly Dictionary<string, string> FilterMap = new()
        {
            { "T", nameof(Filter.Take)},
            { "S", nameof(Filter.Skip)},
            { "O", nameof(Filter.Ordering)},
            { "W", nameof(Filter.WhereClause)},
            { "I", nameof(Filter.Includes)}
        };

        private static readonly Dictionary<string, string> RuleMap = new()
        {
            { "P", nameof(Rule.Path) },
            { "C", nameof(Rule.ComparisonOperator) },
            { "V", nameof(Rule.Value) },
        };

        private static readonly Dictionary<string, string> RuleSetMap = new()
        {
            { "R", nameof(RuleSet.Rules) },
            { "L", nameof(RuleSet.LogicalOperator) },
            { "S", nameof(RuleSet.RuleSets) },
        };

        private static readonly Dictionary<string, string> IncludeMap = new()
        {
            { "P", nameof(Include.Path) },
            { "F", nameof(Include.Filter) }
        };

        public static string GetLongFormPropertyName<TTargetType>(string propertyName)
        {
            if (propertyName.Length > 1)
                return propertyName;

            Dictionary<string, string> map = typeof(TTargetType).Name switch
            {
                nameof(Filter) => FilterMap,
                nameof(Rule) => RuleMap,
                nameof(RuleSet) => RuleSetMap,
                nameof(Include) => IncludeMap,
                _ => throw new JsonException($"{typeof(TTargetType).Name} does not have a property map")
            };

            var shortFormKey = propertyName.ToUpper();

            if (!map.ContainsKey(shortFormKey))
                throw new JsonException($"{propertyName} property can not be mapped on type {typeof(TTargetType).FullName}");

            return map[shortFormKey];
        }
    }
}