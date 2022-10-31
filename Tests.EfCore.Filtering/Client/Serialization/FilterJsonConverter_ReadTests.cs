using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using NUnit.Framework;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class FilterJsonConverter_ReadTests
    {
        [Test]
        public void ThrowJsonExceptionWhenSuppliedJsonDoesNotStartWithStartObjectToken()
        {
            const string json = @"""T"":null";

            var converter = new FilterJsonConverter();

            var ex = Assert.Throws<JsonException>(() =>
            {
                var jsonReader = json.GetJsonReader();
                converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);
            });

            Assert.That(ex.Message, Is.EqualTo("Filter - Object does not start"));
        }

        [Test]
        public void ShortForm_ThrowJsonExceptionWhenPropertyNotFound()
        {
            const int expectedInclude = 1;
            string json = $"{{\"Z\":{expectedInclude}}}";

            var converter = new FilterJsonConverter();

            var ex = Assert.Throws<JsonException>(() =>
            {
                var jsonReader = json.GetJsonReader();
                converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);
            });

            Assert.That(ex.Message, Is.EqualTo("Z property can not be mapped on type EfCore.Filtering.Client.Filter"));
        }

        [Test]
        public void ShortForm_ItReadsTake()
        {
            const int expectedInclude = 1;
            string json = $"{{\"T\":{expectedInclude}}}";

            var converter = new FilterJsonConverter();

            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.That(filter.Take, Is.EqualTo(expectedInclude));
        }

        [Test]
        public void ShortForm_ItReadsTakeWhenNull()
        {
            string json = $"{{\"T\":null}}";

            var converter = new FilterJsonConverter();

            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNull(filter.Take);
        }

        [Test]
        public void LongForm_ItReadsTake()
        {
            const int expectedInclude = 1;
            string json = $"{{\"Take\":{expectedInclude}}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.That(filter.Take, Is.EqualTo(expectedInclude));
        }

        [Test]
        public void LongForm_ItReadsTakeWhenNull()
        {
            string json = $"{{\"Take\":null}}";

            var converter = new FilterJsonConverter();

            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNull(filter.Take);
        }

        [Test]
        public void ShortForm_ItReadsWhenUsingCamelCase()
        {
            const int expectedInclude = 1;
            string json = $"{{\"t\":{expectedInclude}}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.That(filter.Take, Is.EqualTo(expectedInclude));
        }

        [Test]
        public void LongForm_ItReadsWhenUsingCamelCase()
        {
            const int expectedInclude = 1;
            string json = $"{{\"take\":{expectedInclude}}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.That(filter.Take, Is.EqualTo(expectedInclude));
        }

        [Test]
        public void ShortForm_ItReadsSkip()
        {
            const int expectedSkip = 1;
            string json = $"{{\"S\":{expectedSkip}}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.That(filter.Skip, Is.EqualTo(expectedSkip));
        }

        [Test]
        public void LongForm_ItReadsSkip()
        {
            const int expectedSkip = 1;
            string json = $"{{\"Skip\":{expectedSkip}}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.That(filter.Skip, Is.EqualTo(expectedSkip));
        }

        [Test]
        public void ShortForm_ItReadsOrderingAsc()
        {
            const string expectedPath = "Property.Path";
            var json = $"{{\"O\":[\"+{expectedPath}\"]}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter.Ordering);
            Assert.That(filter.Ordering.Count, Is.EqualTo(1));
            Assert.That(filter.Ordering[0].Order, Is.EqualTo(Ordering.ASC));
            Assert.That(filter.Ordering[0].Path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void ShortForm_ItReadsOrderingDesc()
        {
            const string expectedPath = "Property.Path";
            var json = $"{{\"O\":[\"-{expectedPath}\"]}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter.Ordering);
            Assert.That(filter.Ordering.Count, Is.EqualTo(1));
            Assert.That(filter.Ordering[0].Order, Is.EqualTo(Ordering.DESC));
            Assert.That(filter.Ordering[0].Path, Is.EqualTo(expectedPath));
        }

        [Test]
        [TestCase("asc")]
        [TestCase("desc")]
        public void LongForm_ItReadsOrdering(string expectedOrder)
        {
            const string expectedPath = "Property.Path";
            var json = $"{{\"Ordering\":[{{\"Path\":\"{expectedPath}\",\"Order\":\"{expectedOrder}\"}}]}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter.Ordering);
            Assert.That(filter.Ordering.Count, Is.EqualTo(1));
            Assert.That(filter.Ordering[0].Order, Is.EqualTo(expectedOrder));
            Assert.That(filter.Ordering[0].Path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void ShortForm_ItReadsOrderingWithMultipleEntries()
        {
            const string expectedPath1 = "Property.Path";
            const string expectedPath2 = "Property2.Path2";
            var json = $"{{\"O\":[\"+{expectedPath1}\",\"-{expectedPath2}\"]}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter.Ordering);
            Assert.That(filter.Ordering.Count, Is.EqualTo(2));
            Assert.That(filter.Ordering[0].Order, Is.EqualTo(Ordering.ASC));
            Assert.That(filter.Ordering[0].Path, Is.EqualTo(expectedPath1));
            Assert.That(filter.Ordering[1].Order, Is.EqualTo(Ordering.DESC));
            Assert.That(filter.Ordering[1].Path, Is.EqualTo(expectedPath2));
        }

        [Test]
        public void LongForm_ItReadsOrderingWithMultipleEntries()
        {
            const string expectedPath1 = "Property.Path";
            string expectedOrder1 = Ordering.ASC;
            const string expectedPath2 = "Property2.Path2";
            string expectedOrder2 = Ordering.DESC;
            var json = $"{{\"Ordering\":[{{\"Path\":\"{expectedPath1}\",\"Order\":\"{expectedOrder1}\"}},{{\"Path\":\"{expectedPath2}\",\"Order\":\"{expectedOrder2}\"}}]}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter.Ordering);
            Assert.That(filter.Ordering.Count, Is.EqualTo(2));
            Assert.That(filter.Ordering[0].Order, Is.EqualTo(expectedOrder1));
            Assert.That(filter.Ordering[0].Path, Is.EqualTo(expectedPath1));
            Assert.That(filter.Ordering[1].Order, Is.EqualTo(expectedOrder2));
            Assert.That(filter.Ordering[1].Path, Is.EqualTo(expectedPath2));
        }

        [Test]
        public void ShortForm_ItReadsWhere()
        {
            const string propertyPath = "Product.Name";
            const string comparisonEq = "eq";
            const string value = "abc";

            string json = $@"{{""W"": 
                                {{
                                    ""R"":[{{
                                        ""P"": ""{propertyPath}"",
                                        ""C"": ""{comparisonEq}"",
                                        ""V"": ""{value}""
                                    }}]
                                }}
                              }}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter);
            Assert.IsNotNull(filter.WhereClause);
            Assert.IsNotNull(filter.WhereClause.Rules);
            Assert.That(filter.WhereClause.Rules.Count, Is.EqualTo(1));

            var rule = filter.WhereClause.Rules[0];
            Assert.That(rule.Path, Is.EqualTo(propertyPath));
            Assert.That(rule.ComparisonOperator, Is.EqualTo(comparisonEq));
            Assert.IsInstanceOf<JsonElement>(rule.Value);
        }

        [Test]
        public void LongForm_ItReadsWhere()
        {
            const string propertyPath = "Product.Name";
            const string comparisonEq = "eq";
            const string value = "abc";

            string json = $@"{{""WhereClause"": 
                                {{
                                    ""Rules"":[{{
                                        ""Path"": ""{propertyPath}"",
                                        ""ComparisonOperator"": ""{comparisonEq}"",
                                        ""Value"": ""{value}""
                                    }}]
                                }}
                              }}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter);
            Assert.IsNotNull(filter.WhereClause);
            Assert.IsNotNull(filter.WhereClause.Rules);
            Assert.That(filter.WhereClause.Rules.Count, Is.EqualTo(1));

            var rule = filter.WhereClause.Rules[0];
            Assert.That(rule.Path, Is.EqualTo(propertyPath));
            Assert.That(rule.ComparisonOperator, Is.EqualTo(comparisonEq));
            Assert.IsInstanceOf<JsonElement>(rule.Value);
        }

        [Test]
        public void ShortForm_ItReadsWhereWithMultipleRules()
        {
            const string propertyPath = "Product.Name";
            const string comparisonEq = "eq";
            const string value = "abc";

            const string propertyPath2 = "Product.Listings.AnotherValue";
            const string comparisonEq2 = "eq";
            const int value2 = 3;

            string json = $@"{{""W"": 
                                {{
                                    ""R"":[{{
                                        ""P"": ""{propertyPath}"",
                                        ""C"": ""{comparisonEq}"",
                                        ""V"": ""{value}""
                                    }},{{
                                        ""P"": ""{propertyPath2}"",
                                        ""C"": ""{comparisonEq2}"",
                                        ""V"": {value2}
                                    }}]
                                }}
                              }}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter);
            Assert.IsNotNull(filter.WhereClause);
            Assert.IsNotNull(filter.WhereClause.Rules);
            Assert.That(filter.WhereClause.Rules.Count, Is.EqualTo(2));

            var rule = filter.WhereClause.Rules[0];
            Assert.That(rule.Path, Is.EqualTo(propertyPath));
            Assert.That(rule.ComparisonOperator, Is.EqualTo(comparisonEq));
            Assert.IsInstanceOf<JsonElement>(rule.Value);

            rule = filter.WhereClause.Rules[1];
            Assert.That(rule.Path, Is.EqualTo(propertyPath2));
            Assert.That(rule.ComparisonOperator, Is.EqualTo(comparisonEq2));
            Assert.IsInstanceOf<JsonElement>(rule.Value);
        }
     
        [Test]
        public void ShortForm_ItReadsInclude()
        {
            const string expectedPath = "ShopListings";
            string json = $"{{\"i\":[{{\"p\":\"{expectedPath}\"}}]}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter);
            Assert.IsNotNull(filter.Includes);
            Assert.That(filter.Includes.Count, Is.EqualTo(1));
            Assert.That(filter.Includes[0].Path, Is.EqualTo(expectedPath));               
        }

        [Test]
        public void LongForm_ItReadsInclude()
        {
            const string expectedPath = "ShopListings";
            string json = $"{{\"includes\":[{{\"path\":\"{expectedPath}\"}}]}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter);
            Assert.IsNotNull(filter.Includes);
            Assert.That(filter.Includes.Count, Is.EqualTo(1));
            Assert.That(filter.Includes[0].Path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void ShortForm_ItReadsMultipleInclude()
        {
            const string expectedPath1 = "Shop";
            const string expectedPath2 = "Product";
            string json = $"{{\"i\":[{{\"p\":\"{expectedPath1}\"}}, {{\"p\":\"{expectedPath2}\"}}]}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter);
            Assert.IsNotNull(filter.Includes);
            Assert.That(filter.Includes.Count, Is.EqualTo(2));
            Assert.That(filter.Includes[0].Path, Is.EqualTo(expectedPath1));
            Assert.That(filter.Includes[1].Path, Is.EqualTo(expectedPath2));
        }

        [Test]
        public void LongForm_ItReadsMultipleInclude()
        {

            const string expectedPath1 = "Shop";
            const string expectedPath2 = "Product";
            string json = $"{{\"includes\":[{{\"path\":\"{expectedPath1}\"}}, {{\"path\":\"{expectedPath2}\"}}]}}";

            var converter = new FilterJsonConverter();
            var jsonReader = json.GetJsonReader();
            var filter = converter.Read(ref jsonReader, typeof(Filter), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(filter);
            Assert.IsNotNull(filter.Includes);
            Assert.That(filter.Includes.Count, Is.EqualTo(2));
            Assert.That(filter.Includes[0].Path, Is.EqualTo(expectedPath1));
            Assert.That(filter.Includes[1].Path, Is.EqualTo(expectedPath2));
        }
    }
}
