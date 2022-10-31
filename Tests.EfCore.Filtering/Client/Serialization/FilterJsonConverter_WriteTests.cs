using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class FilterJsonConverter_WriteTests
    {

        [Test]
        public void ItWritesFullFilter()
        {
            var filter = new Filter
            {
                Skip = 1,
                Take = 10,
                WhereClause = new RuleSet
                {
                    Rules = new List<Rule>
                    {
                        new Rule
                        {
                            ComparisonOperator = ComparisonOperators.Equal,
                            Path = "This.Is.My.Path",
                            Value = 234
                        }
                    }
                },
                Includes = new List<Include>
                {
                    new Include { Path = "This.Yes" },
                    new Include
                    {
                        Path = "This.Boo",
                        Filter = new Filter
                        {
                            Take = 10,
                        }
                    },
                },
                Ordering = new List<OrderBy>
                {
                    new OrderBy { Path = "This.Boo" },
                    new OrderBy { Path = "This.Yes", Order = Ordering.DESC }
                }
            };

            var expectedJson = @$"{{
                    ""I"":[{{
                        ""P"":""{filter.Includes[0].Path}""
                    }},{{
                        ""F"":{{
                            ""T"":{filter.Includes[1].Filter.Take}
                         }},
                        ""P"":""{filter.Includes[1].Path}""
                    }}],
                    ""O"":[""\u002B{filter.Ordering[0].Path}"",""-{filter.Ordering[1].Path}""],
                    ""S"":{filter.Skip},
                    ""T"":{filter.Take},
                    ""W"":{{
                        ""R"":[{{
                                ""C"":""{filter.WhereClause.Rules[0].ComparisonOperator}"",
                                ""P"":""{filter.WhereClause.Rules[0].Path}"",
                                ""V"":{filter.WhereClause.Rules[0].Value}
                            }}]
                    }}
                }}";

            expectedJson = expectedJson.Replace(Environment.NewLine, "").Replace(" ", "");

            var converter = new FilterJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, filter, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}
