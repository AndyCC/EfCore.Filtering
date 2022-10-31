using EfCore.Filtering.Client;
using EfCore.Filtering.Paths;
using System;
using System.Text.Json;

namespace EfCore.Filtering.Mvc
{
    /// <summary>
    /// Extension methods for all rule values within a filter
    /// </summary>
    public static class FilterRuleValueHelpers
    {
        /// <summary>
        /// Changes any rule valuse from JsonElement to correct type
        /// </summary>        /// 
        /// <typeparam name="TSourceType">Type of object the filter will be applied to</typeparam>
        /// <param name="filter">filter to change rules on</param>
        /// <param name="pathWalker">PathWalker containing the evaluated paths in the filter</param>
        public static void EnsureRuleValuesAreCorrectType<TSourceType>(this Filter filter, PathWalker<TSourceType> pathWalker)
        {
            filter.EnsureRuleValuesAreCorrectType(typeof(TSourceType), pathWalker);
        }

        /// <summary>
        /// Changes any rule valuse from that are JsoneElements of strings are converted to the correct type
        /// </summary>
        /// <param name="filter">filter to change rules on</param>
        /// <param name="sourceType">Type of object the filter will be applied to</param>
        /// <param name="pathWalker">PathWalker containing the evaluated paths in the filter</param>
        public static void EnsureRuleValuesAreCorrectType(this Filter filter, Type sourceType, PathWalker pathWalker)
        {
            if (filter.HasWhereClauseRules)
                RuleValuesToActualType(filter.WhereClause, sourceType, pathWalker);

            if (filter.Includes == null)            
                return;            

            foreach (var includeFilter in filter.Includes)
            {
                var propertyType = pathWalker.GetFinalTypeOfFinalPropertyInPath(sourceType, includeFilter.Path);

                if (includeFilter.Filter != null)
                {
                    propertyType = propertyType.GetUnderlyingTypeIfGenericAndEnumerable() ?? propertyType;
                    includeFilter.Filter.EnsureRuleValuesAreCorrectType(propertyType, pathWalker);
                }
            }
        }

        /// <summary>
        /// Changes all values on rules within a rule set to their actual types from JsonElements or from string values
        /// </summary>
        /// <param name="ruleSet">RuleSet</param>
        /// <param name="sourceType">Type the filter is to be applied to</param>
        /// <param name="pathWalker">PathWalker containing the evaluated paths in the filter</param>
        private static void RuleValuesToActualType(RuleSet ruleSet, Type sourceType, PathWalker pathWalker)
        {
            foreach (var rule in ruleSet.Rules)
            {
                try
                {
                    var targetType = pathWalker.GetFinalTypeOfFinalPropertyInPath(sourceType, rule.Path);

                    if (rule.Value.GetType() == typeof(JsonElement))
                        rule.ChangeRuleValueFromJsonToType(targetType);
                    else if (rule.Value.GetType() == typeof(string))
                        rule.ChangeRuleValueFromStringToType(targetType);
                }
                catch (Exception ex)
                {
                    throw new FilterRuleValueChangeException($"Can not change rule type on rule with path {rule.Path} and operator {rule.ComparisonOperator}", ex);
                }
            }

            if (ruleSet.RuleSets == null)
                return;

            foreach (var innerRuleSet in ruleSet.RuleSets)
                RuleValuesToActualType(innerRuleSet, sourceType, pathWalker);
        }

        /// <summary>
        /// Changes a rule's value from json to the required type
        /// </summary>
        /// <param name="rule">Rule</param>
        /// <param name="targetType">Type the rule value should be</param>
        private static void ChangeRuleValueFromJsonToType(this Rule rule, Type targetType)
        {
            var jsonElement = (JsonElement)rule.Value;
            var json = jsonElement.GetRawText();

            if (jsonElement.ValueKind == JsonValueKind.Array)
                targetType = Type.GetType($"{targetType.FullName}[], {targetType.Assembly.FullName}");

            rule.Value = JsonSerializer.Deserialize(json, targetType);
        }

        /// <summary>
        ///  Changes a rule's value from string to the required type
        /// </summary>
        /// <param name="rule">Rule</param>
        /// <param name="targetType">Type the rule value should be</param>
        private static void ChangeRuleValueFromStringToType(this Rule rule, Type targetType)
        {
            var rawValue = (string)rule.Value;

            if (rawValue.StartsWith("[") && rawValue.EndsWith("]"))
            {
                var rawItems = rawValue.Substring(1, rawValue.Length - 2).Split(",");
                var array = (object[])Array.CreateInstance(targetType, rawItems.Length);

                for (var i = 0; i < rawItems.Length; i++)
                    array[i] = Convert.ChangeType(rawItems[i], targetType);

                rule.Value = array;
            }
            else
                rule.Value = Convert.ChangeType(rule.Value, targetType);
        }
    }
}
