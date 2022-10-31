using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class RuleJsonConverter_ReadTests
    {
        [Test]
        [TestCaseSource(nameof(Data_Values))]
        public void ShortForm_ItReadsRuleWithValue(object expectedValue, JsonValueKind expectedValueKind)
        {
            const string expectedPath = "Property.Path";
            const string expectedComparisonOp = "eq";

            var quote = expectedValue is string ? "\"" : string.Empty;
            var jsonValue = expectedValue == null ? "null" : expectedValue;
            string json = $"{{\"P\":\"{expectedPath}\",\"C\":\"{expectedComparisonOp}\",\"V\":{quote}{jsonValue}{quote}}}";

            var converter = new RuleJsonConverter();
            var jsonReader = json.GetJsonReader();
            
            var rule = converter.Read(ref jsonReader, typeof(Rule), SerializationTestHelpers.SerializeOptions);
            Assert.IsNotNull(rule);
            Assert.That(rule.Path, Is.EqualTo(expectedPath));

            if (expectedValue == null)
                Assert.IsNull(rule.Value);
            else
            {
                Assert.IsInstanceOf<JsonElement>(rule.Value);
                var jsonEl = (JsonElement)rule.Value;
                Assert.That(jsonEl.ValueKind, Is.EqualTo(expectedValueKind));
             }

            Assert.That(rule.ComparisonOperator, Is.EqualTo(expectedComparisonOp));
        }

        [Test]
        [TestCaseSource(nameof(Data_Values))]
        public void LongForm_ItReadsRuleWithValue(object expectedValue, JsonValueKind expectedValueKind)
        {
            const string expectedPath = "Property.Path";
            const string expectedComparisonOp = "eq";

            var quote = expectedValue is string ? "\"" : string.Empty;
            var jsonValue = expectedValue == null ? "null" : expectedValue;
            string json = $"{{\"Path\":\"{expectedPath}\",\"ComparisonOperator\":\"{expectedComparisonOp}\",\"Value\":{quote}{jsonValue}{quote}}}";

            var converter = new RuleJsonConverter();
            var jsonReader = json.GetJsonReader();

            var rule = converter.Read(ref jsonReader, typeof(Rule), SerializationTestHelpers.SerializeOptions);
            Assert.IsNotNull(rule);
            Assert.That(rule.Path, Is.EqualTo(expectedPath));

            if (expectedValue == null)
                Assert.IsNull(rule.Value);
            else
            {
                Assert.IsInstanceOf<JsonElement>(rule.Value);
                var jsonEl = (JsonElement)rule.Value;
                Assert.That(jsonEl.ValueKind, Is.EqualTo(expectedValueKind));
            }

            Assert.That(rule.ComparisonOperator, Is.EqualTo(expectedComparisonOp));
        }

        private static IEnumerable<object[]> Data_Values
        {
            get
            {
                yield return new object[] { 2, JsonValueKind.Number };
                yield return new object[] { 2.1m, JsonValueKind.Number };
                yield return new object[] { 2.1f, JsonValueKind.Number };
                yield return new object[] { 2.1, JsonValueKind.Number };
                yield return new object[] { "a value", JsonValueKind.String };
                yield return new object[] { null, default };
            }
        }

        [Test]
        [TestCaseSource(typeof(SerializationTestHelpers), nameof(SerializationTestHelpers.RuleComparisonOperators))]
        public void ShortForm_ItReadsOpertors(string expectedComparisonOp)
        {
            const string expectedPath = "Property.Path";

            const int expectedValue = 1;
            string json = $"{{\"P\":\"{expectedPath}\",\"C\":\"{expectedComparisonOp}\",\"V\":{expectedValue}}}";

            var converter = new RuleJsonConverter();
            var jsonReader = json.GetJsonReader();

            var rule = converter.Read(ref jsonReader, typeof(Rule), SerializationTestHelpers.SerializeOptions);
            Assert.IsNotNull(rule);
            Assert.That(rule.Path, Is.EqualTo(expectedPath));
            Assert.That(rule.ComparisonOperator, Is.EqualTo(expectedComparisonOp));
        }

        [Test]
        [TestCaseSource(typeof(SerializationTestHelpers), nameof(SerializationTestHelpers.RuleComparisonOperators))]
        public void LongForm_ItReadsOperators(string expectedComparisonOp)
        {
            const string expectedPath = "Property.Path";
            const int expectedValue = 1;

            string json = $"{{\"Path\":\"{expectedPath}\",\"ComparisonOperator\":\"{expectedComparisonOp}\",\"Value\":{expectedValue}}}";

            var converter = new RuleJsonConverter();
            var jsonReader = json.GetJsonReader();

            var rule = converter.Read(ref jsonReader, typeof(Rule), SerializationTestHelpers.SerializeOptions);
            Assert.IsNotNull(rule);
            Assert.That(rule.Path, Is.EqualTo(expectedPath));
            Assert.That(rule.ComparisonOperator, Is.EqualTo(expectedComparisonOp));
        }

        [Test]
        public void ShortForm_ItReadsRuleWithIntArrayValue()
        {
            const string expectedPath = "Property.Path";
            const string expectedComparisonOp = "eq";

            string json = $"{{\"P\":\"{expectedPath}\",\"C\":\"{expectedComparisonOp}\",\"V\":[1,2]}}";

            var converter = new RuleJsonConverter();
            var jsonReader = json.GetJsonReader();

            var rule = converter.Read(ref jsonReader, typeof(Rule), SerializationTestHelpers.SerializeOptions);
            Assert.IsNotNull(rule);
            Assert.That(rule.Path, Is.EqualTo(expectedPath));
            Assert.IsNotNull(rule.Value);
            Assert.IsInstanceOf<JsonElement>(rule.Value);
            var jsonEl = (JsonElement)rule.Value;
            Assert.That(jsonEl.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(rule.ComparisonOperator, Is.EqualTo(expectedComparisonOp));
        }

        [Test]
        public void ShortForm_ItReadsRuleWithStringArrayValue()
        {
            const string expectedPath = "Property.Path";
            const string expectedComparisonOp = "eq";

            string json = $"{{\"P\":\"{expectedPath}\",\"C\":\"{expectedComparisonOp}\",\"V\":[\"A\",\"B\"]}}";

            var converter = new RuleJsonConverter();
            var jsonReader = json.GetJsonReader();

            var rule = converter.Read(ref jsonReader, typeof(Rule), SerializationTestHelpers.SerializeOptions);
            Assert.IsNotNull(rule);
            Assert.That(rule.Path, Is.EqualTo(expectedPath));
            Assert.IsNotNull(rule.Value);
            Assert.IsInstanceOf<JsonElement>(rule.Value);
            var jsonEl = (JsonElement)rule.Value;
            Assert.That(jsonEl.ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(rule.ComparisonOperator, Is.EqualTo(expectedComparisonOp));
        }
    }
}
