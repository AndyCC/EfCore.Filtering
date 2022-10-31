using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using NUnit.Framework;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class RuleSetJsonConverter_ReadTests
    {
        [Test]
        public void ShortForm_ItShouldReadARuleSetWithOneRule()
        {
            const string expectedPath = "Property.Path";
            const string expectedComparisonOp = "eq";
            const int value = 123;

            var json = $"{{\"R\":[{{\"P\":\"{expectedPath}\",\"C\":\"{expectedComparisonOp}\",\"V\":{value}}}]}}";

            var converter = new RuleSetJsonConverter();
            var jsonReader = json.GetJsonReader();

            var ruleSet = converter.Read(ref jsonReader, typeof(RuleSet), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(ruleSet);
            Assert.IsNotNull(ruleSet.Rules);
            Assert.That(ruleSet.Rules.Count, Is.EqualTo(1));

            Assert.That(ruleSet.Rules[0].Path, Is.EqualTo(expectedPath));
            Assert.That(ruleSet.Rules[0].ComparisonOperator, Is.EqualTo(expectedComparisonOp));
            Assert.IsInstanceOf<JsonElement>(ruleSet.Rules[0].Value);
        }

        [Test]
        public void LongForm_ItShouldReadARuleSetWithOneRule()
        {
            const string expectedPath = "Property.Path";
            const string expectedComparisonOp = "eq";
            const int value = 123;

            var json = $"{{\"Rules\":[{{\"Path\":\"{expectedPath}\",\"ComparisonOperator\":\"{expectedComparisonOp}\",\"Value\":{value}}}]}}";

            var converter = new RuleSetJsonConverter();
            var jsonReader = json.GetJsonReader();

            var ruleSet = converter.Read(ref jsonReader, typeof(RuleSet), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(ruleSet);
            Assert.IsNotNull(ruleSet.Rules);
            Assert.That(ruleSet.Rules.Count, Is.EqualTo(1));

            Assert.That(ruleSet.Rules[0].Path, Is.EqualTo(expectedPath));
            Assert.That(ruleSet.Rules[0].ComparisonOperator, Is.EqualTo(expectedComparisonOp));
            Assert.IsInstanceOf<JsonElement>(ruleSet.Rules[0].Value);
        }

        [Test]
        [TestCase("A", "AND")]
        [TestCase("O", "OR")]
        public void ShortForm_ItShouldReadARuleSetWithTwoRulesAndALogicalOperator(string logicalOp, string expectedLogicalOperator)
        {
            const string expectedPath1 = "Property.Path";
            const string expectedComparisonOp1 = "eq";
            const string expectedPath2 = "Property2.Path2";
            const string expectedComparisonOp2 = "lt";
            const int value = 123;

            var json = @$"{{
                            ""L"":""{logicalOp}"",
                            ""R"":[{{
                                ""P"":""{expectedPath1}"",
                                ""C"":""{expectedComparisonOp1}"",
                                ""V"":{value}
                             }},{{
                                ""P"":""{expectedPath2}"",
                                ""C"":""{expectedComparisonOp2}"",
                                ""V"":{value}
                             }}]
                           }}";

            var converter = new RuleSetJsonConverter();
            var jsonReader = json.GetJsonReader();

            var ruleSet = converter.Read(ref jsonReader, typeof(RuleSet), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(ruleSet);
            Assert.That(ruleSet.LogicalOperator, Is.EqualTo(expectedLogicalOperator));
            Assert.IsNotNull(ruleSet.Rules);
            Assert.That(ruleSet.Rules.Count, Is.EqualTo(2));

            Assert.That(ruleSet.Rules[0].Path, Is.EqualTo(expectedPath1));
            Assert.That(ruleSet.Rules[0].ComparisonOperator, Is.EqualTo(expectedComparisonOp1));
            Assert.IsInstanceOf<JsonElement>(ruleSet.Rules[0].Value);

            Assert.That(ruleSet.Rules[1].Path, Is.EqualTo(expectedPath2));
            Assert.That(ruleSet.Rules[1].ComparisonOperator, Is.EqualTo(expectedComparisonOp2));
            Assert.IsInstanceOf<JsonElement>(ruleSet.Rules[1].Value);
        }

        [Test]
        [TestCase("AND")]
        [TestCase("OR")]
        public void LongForm_ItShouldReadARuleSetWithTwoRulesAndALogicalOperator(string expectedLogicalOperator)
        {
            const string expectedPath1 = "Property.Path";
            const string expectedComparisonOp1 = "eq";
            const string expectedPath2 = "Property2.Path2";
            const string expectedComparisonOp2 = "lt";
            const int value = 123;

            var json = @$"{{
                            ""LogicalOperator"":""{expectedLogicalOperator}"",
                            ""Rules"":[{{
                                ""Path"":""{expectedPath1}"",
                                ""ComparisonOperator"":""{expectedComparisonOp1}"",
                                ""Value"":{value}
                             }},{{
                                ""Path"":""{expectedPath2}"",
                                ""ComparisonOperator"":""{expectedComparisonOp2}"",
                                ""Value"":{value}
                             }}]
                           }}";

            var converter = new RuleSetJsonConverter();
            var jsonReader = json.GetJsonReader();

            var ruleSet = converter.Read(ref jsonReader, typeof(RuleSet), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(ruleSet);
            Assert.That(ruleSet.LogicalOperator, Is.EqualTo(expectedLogicalOperator));
            Assert.IsNotNull(ruleSet.Rules);
            Assert.That(ruleSet.Rules.Count, Is.EqualTo(2));

            Assert.That(ruleSet.Rules[0].Path, Is.EqualTo(expectedPath1));
            Assert.That(ruleSet.Rules[0].ComparisonOperator, Is.EqualTo(expectedComparisonOp1));
            Assert.IsInstanceOf<JsonElement>(ruleSet.Rules[0].Value);

            Assert.That(ruleSet.Rules[1].Path, Is.EqualTo(expectedPath2));
            Assert.That(ruleSet.Rules[1].ComparisonOperator, Is.EqualTo(expectedComparisonOp2));
            Assert.IsInstanceOf<JsonElement>(ruleSet.Rules[1].Value);
        }

        [Test]
        public void ShortForm_ItShouldThrowJsonExceptionOnUnSupportedLogicalOperator()
        {
            const string expectedPath1 = "Property.Path";
            const string expectedComparisonOp1 = "eq";
            const string expectedPath2 = "Property2.Path2";
            const string expectedComparisonOp2 = "lt";
            const int value = 123;

            var json = @$"{{
                            ""L"":""N"",
                            ""R"":[{{
                                ""P"":""{expectedPath1}"",
                                ""C"":""{expectedComparisonOp1}"",
                                ""V"":{value}
                             }},{{
                                ""P"":""{expectedPath2}"",
                                ""C"":""{expectedComparisonOp2}"",
                                ""V"":{value}
                             }}]
                           }}";

            var exception = Assert.Throws<JsonException>(() =>
            {
                var converter = new RuleSetJsonConverter();
                var jsonReader = json.GetJsonReader();
                converter.Read(ref jsonReader, typeof(RuleSet), SerializationTestHelpers.SerializeOptions);
            });

            Assert.That(exception.Message, Is.EqualTo("Unsuported Logical Operator N"));
        }

        [Test]
        public void LongForm_ItShouldThrowJsonExceptionOnUnSupportedLogicalOperator()
        {
            const string expectedPath1 = "Property.Path";
            const string expectedComparisonOp1 = "eq";
            const string expectedPath2 = "Property2.Path2";
            const string expectedComparisonOp2 = "lt";
            const int value = 123;

            var json = @$"{{
                            ""LogicalOperator"":""N"",
                            ""Rules"":[{{
                                ""Path"":""{expectedPath1}"",
                                ""ComparisonOperator"":""{expectedComparisonOp1}"",
                                ""Value"":{value}
                             }},{{
                                ""Path"":""{expectedPath2}"",
                                ""ComparisonOperator"":""{expectedComparisonOp2}"",
                                ""Value"":{value}
                             }}]
                           }}";

            var exception = Assert.Throws<JsonException>(() =>
            {
                var converter = new RuleSetJsonConverter();
                var jsonReader = json.GetJsonReader();
                converter.Read(ref jsonReader, typeof(RuleSet), SerializationTestHelpers.SerializeOptions);
            });

            Assert.That(exception.Message, Is.EqualTo("Unsuported Logical Operator N"));
        }

        [Test]
        [TestCase("A", "AND", "O", "OR")]
        [TestCase("O", "OR", "A", "AND")]
        public void ShortForm_ItShouldReadARuleSetWithASubRuleSet(string logicalOp1, string expectedLogicalOperator1, 
            string logicalOp2, string expectedLogicalOperator2)
        {
            const string expectedPath1 = "Property.Path";
            const string expectedComparisonOp1 = "eq";
            const string expectedPath2 = "Property2.Path2";
            const string expectedComparisonOp2 = "lt";
            const string expectedPath3 = "Property3.Path3";
            const string expectedComparisonOp3 = "gt";
            const int value = 123;

            var json = @$"{{
                            ""L"":""{logicalOp1}"",
                            ""R"":[{{
                                ""P"":""{expectedPath1}"",
                                ""C"":""{expectedComparisonOp1}"",
                                ""V"":{value}
                             }}], 
                            ""S"":[{{
                               ""L"":""{logicalOp2}"",
                               ""R"": [{{
                                   ""P"":""{expectedPath2}"",
                                   ""C"":""{expectedComparisonOp2}"",
                                   ""V"":{value}
                                }}, {{
                                   ""P"":""{expectedPath3}"",
                                   ""C"":""{expectedComparisonOp3}"",
                                   ""V"":{value}
                                }}]
                            }}]
                           }}";

            var converter = new RuleSetJsonConverter();
            var jsonReader = json.GetJsonReader();

            var ruleSet = converter.Read(ref jsonReader, typeof(RuleSet), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(ruleSet);
            Assert.That(ruleSet.LogicalOperator, Is.EqualTo(expectedLogicalOperator1));
            Assert.IsNotNull(ruleSet.Rules);
            Assert.That(ruleSet.Rules.Count, Is.EqualTo(1));

            Assert.That(ruleSet.Rules[0].Path, Is.EqualTo(expectedPath1));
            Assert.That(ruleSet.Rules[0].ComparisonOperator, Is.EqualTo(expectedComparisonOp1));
            Assert.IsInstanceOf<JsonElement>(ruleSet.Rules[0].Value);

            Assert.IsNotNull(ruleSet.RuleSets);
            Assert.That(ruleSet.RuleSets.Count, Is.EqualTo(1));

            var subRuleSet = ruleSet.RuleSets[0];
            Assert.That(subRuleSet.LogicalOperator, Is.EqualTo(expectedLogicalOperator2));
            Assert.That(subRuleSet.Rules.Count, Is.EqualTo(2));

            Assert.That(subRuleSet.Rules[0].Path, Is.EqualTo(expectedPath2));
            Assert.That(subRuleSet.Rules[0].ComparisonOperator, Is.EqualTo(expectedComparisonOp2));
            Assert.IsInstanceOf<JsonElement>(subRuleSet.Rules[0].Value);

            Assert.That(subRuleSet.Rules[1].Path, Is.EqualTo(expectedPath3));
            Assert.That(subRuleSet.Rules[1].ComparisonOperator, Is.EqualTo(expectedComparisonOp3));
            Assert.IsInstanceOf<JsonElement>(subRuleSet.Rules[1].Value);
        }

        [Test]
        [TestCase("AND", "OR")]
        [TestCase("OR", "AND")]
        public void LongForm_ItShouldReadARuleSetWithASubRuleSet(string expectedLogicalOperator1, string expectedLogicalOperator2)
        {
            const string expectedPath1 = "Property.Path";
            const string expectedComparisonOp1 = "eq";
            const string expectedPath2 = "Property2.Path2";
            const string expectedComparisonOp2 = "lt";
            const string expectedPath3 = "Property3.Path3";
            const string expectedComparisonOp3 = "gt";
            const int value = 123;

            var json = @$"{{
                            ""LogicalOperator"":""{expectedLogicalOperator1}"",
                            ""Rules"":[{{
                                ""Path"":""{expectedPath1}"",
                                ""ComparisonOperator"":""{expectedComparisonOp1}"",
                                ""Value"":{value}
                             }}], 
                            ""RuleSets"":[{{
                               ""LogicalOperator"":""{expectedLogicalOperator2}"",
                               ""Rules"": [{{
                                   ""Path"":""{expectedPath2}"",
                                   ""ComparisonOperator"":""{expectedComparisonOp2}"",
                                   ""Value"":{value}
                                }}, {{
                                   ""Path"":""{expectedPath3}"",
                                   ""ComparisonOperator"":""{expectedComparisonOp3}"",
                                   ""Value"":{value}
                                }}]
                            }}]
                           }}";

            var converter = new RuleSetJsonConverter();
            var jsonReader = json.GetJsonReader();

            var ruleSet = converter.Read(ref jsonReader, typeof(RuleSet), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(ruleSet);
            Assert.That(ruleSet.LogicalOperator, Is.EqualTo(expectedLogicalOperator1));
            Assert.IsNotNull(ruleSet.Rules);
            Assert.That(ruleSet.Rules.Count, Is.EqualTo(1));

            Assert.That(ruleSet.Rules[0].Path, Is.EqualTo(expectedPath1));
            Assert.That(ruleSet.Rules[0].ComparisonOperator, Is.EqualTo(expectedComparisonOp1));
            Assert.IsInstanceOf<JsonElement>(ruleSet.Rules[0].Value);

            Assert.IsNotNull(ruleSet.RuleSets);
            Assert.That(ruleSet.RuleSets.Count, Is.EqualTo(1));

            var subRuleSet = ruleSet.RuleSets[0];
            Assert.That(subRuleSet.LogicalOperator, Is.EqualTo(expectedLogicalOperator2));
            Assert.That(subRuleSet.Rules.Count, Is.EqualTo(2));

            Assert.That(subRuleSet.Rules[0].Path, Is.EqualTo(expectedPath2));
            Assert.That(subRuleSet.Rules[0].ComparisonOperator, Is.EqualTo(expectedComparisonOp2));
            Assert.IsInstanceOf<JsonElement>(subRuleSet.Rules[0].Value);

            Assert.That(subRuleSet.Rules[1].Path, Is.EqualTo(expectedPath3));
            Assert.That(subRuleSet.Rules[1].ComparisonOperator, Is.EqualTo(expectedComparisonOp3));
            Assert.IsInstanceOf<JsonElement>(subRuleSet.Rules[1].Value);
        }
    }
}
