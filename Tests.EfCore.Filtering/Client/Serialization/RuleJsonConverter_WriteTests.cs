using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using NUnit.Framework;
using System.IO;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class RuleJsonConverter_WriteTests
    {
        [Test]
        [TestCaseSource(typeof(SerializationTestHelpers), nameof(SerializationTestHelpers.RuleComparisonOperators))]
        public void ItWritesShortFormRule(string expectedComparisonOp)
        {
            const string expectedPath = "Property.Path";

            var expectedValue = 1; 

            var expectedJson = @$"{{""C"":""{expectedComparisonOp}"",""P"":""{expectedPath}"",""V"":{expectedValue}}}";

            var rule = new Rule {
                ComparisonOperator = expectedComparisonOp,
                Path = expectedPath,
                Value = expectedValue
            };

            var converter = new RuleJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, rule, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void ItWritesArray()
        {
            const string expectedPath = "Property.Path";
            const string expectedComparisonOp = "in";

            var expectedValue = new string[] { "A", "B", "C" };

            var expectedJson = @$"{{""C"":""{expectedComparisonOp}"",""P"":""{expectedPath}"",""V"":[""A"",""B"",""C""]}}";

            var rule = new Rule
            {
                ComparisonOperator = expectedComparisonOp,
                Path = expectedPath,
                Value = expectedValue
            };

            var converter = new RuleJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, rule, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}
