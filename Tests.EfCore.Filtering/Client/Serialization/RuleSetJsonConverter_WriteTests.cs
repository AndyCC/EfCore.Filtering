using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class RuleSetJsonConverter_WriteTests
    {
        [Test]
        public void ItWritesRuleSetWithBasicRule()
        {
            const string expectedPath = "Property.Path";
            const string expectedComparisonOp = "eq";
            const int value = 123;

            var expectedJson = $"{{\"R\":[{{\"C\":\"{expectedComparisonOp}\",\"P\":\"{expectedPath}\",\"V\":{value}}}]}}";

            var ruleSet = new RuleSet
            {
                Rules = new List<Rule>
                {
                    new Rule
                    {
                        ComparisonOperator = expectedComparisonOp,
                        Path = expectedPath,
                        Value = value
                    }
                }
            };

            var converter = new RuleSetJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, ruleSet, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        [TestCase("A", "AND")]
        [TestCase("O", "OR")]
        public void ItWritesRuleSetWithMultipleRules(string expectedLogicalOp, string initialLogicalOp)
        {
            const string expectedPath1 = "Property.Path";
            const string expectedComparisonOp1 = "eq";
            const string value1 = "astring";
            const string expectedPath2 = "Property2.Path2";
            const string expectedComparisonOp2 = "lt";
            const int value2 = 243;

            var expectedJson = $"{{\"L\":\"{expectedLogicalOp}\",\"R\":[{{\"C\":\"{expectedComparisonOp1}\",\"P\":\"{expectedPath1}\",\"V\":\"{value1}\"}},{{\"C\":\"{expectedComparisonOp2}\",\"P\":\"{expectedPath2}\",\"V\":{value2}}}]}}";

            var ruleSet = new RuleSet
            {
                LogicalOperator = initialLogicalOp,
                Rules = new List<Rule>
                {                   
                    new Rule
                    {
                        ComparisonOperator = expectedComparisonOp1,
                        Path = expectedPath1,
                        Value = value1
                    },
                    new Rule
                    {
                        ComparisonOperator = expectedComparisonOp2,
                        Path = expectedPath2,
                        Value = value2
                    }
                }
            };

            var converter = new RuleSetJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, ruleSet, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        [TestCase("A", "AND", "O", "OR")]
        [TestCase("O", "OR", "A", "AND")]
        public void ItWritesRuleSetWithNestedRuleSett(string expectedLogicalOperator1, string logicalOp1,
            string expectedLogicalOperator2, string logicalOp2)
        {
            const string expectedPath1 = "Property.Path";
            const string expectedComparisonOp1 = "eq";
            const string expectedPath2 = "Property2.Path2";
            const string expectedComparisonOp2 = "lt";
            const string expectedPath3 = "Property3.Path3";
            const string expectedComparisonOp3 = "gt";
            const int value = 123;

            var expectedJson = @$"{{
                            ""L"":""{expectedLogicalOperator1}"",
                            ""R"":[{{
                                ""C"":""{expectedComparisonOp1}"",
                                ""P"":""{expectedPath1}"",
                                ""V"":{value}
                             }}], 
                            ""S"":[{{
                               ""L"":""{expectedLogicalOperator2}"",
                               ""R"": [{{
                                   ""C"":""{expectedComparisonOp2}"",
                                   ""P"":""{expectedPath2}"",
                                   ""V"":{value}
                                }}, {{
                                   ""C"":""{expectedComparisonOp3}"",
                                   ""P"":""{expectedPath3}"",
                                   ""V"":{value}
                                }}]
                            }}]
                           }}";

            expectedJson = expectedJson.Replace(Environment.NewLine, "")
                .Replace(" ", "");

            var ruleSet = new RuleSet
            {
                LogicalOperator = logicalOp1,
                Rules = new List<Rule>
                {
                    new Rule
                    {
                        Path  = expectedPath1,
                        ComparisonOperator = expectedComparisonOp1,
                        Value = value
                    }
                },
                RuleSets = new List<RuleSet>
                {
                    new RuleSet
                    {
                        LogicalOperator = logicalOp2,
                        Rules = new List<Rule>
                        {
                            new Rule
                            {
                                Path = expectedPath2,
                                ComparisonOperator = expectedComparisonOp2,
                                Value = value
                            },
                            new Rule
                            {
                                Path = expectedPath3,
                                ComparisonOperator = expectedComparisonOp3,
                                Value = value
                            }
                        }
                    }
                }
            };

            var converter = new RuleSetJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, ruleSet, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}
