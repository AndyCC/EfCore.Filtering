using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class IncludeJsonConverter_WriteTests
    {
        [Test]
        public void ItWritesIncludeWithoutFilter()
        {
            const string expectedPath = "Property.Path";
            const string expectedJson = $"{{\"P\":\"{expectedPath}\"}}";

            var include = new Include
            {
                Path = expectedPath,
            };

            var converter = new IncludeJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, include, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void ItWritesIncludeWithFilter()
        {
            var include = new Include
            {
                Path = "Property.Path",
                Filter = new Filter
                {
                    Skip = 1,
                    Take = 10,
                    Ordering = new List<OrderBy>
                    {
                        new OrderBy
                        {
                            Order = Ordering.DESC,
                            Path = "Id"
                        }
                    }
                }
            };

            var expectedJson = @$"{{
                ""F"": {{
                   ""O"":[""-{include.Filter.Ordering[0].Path}""],
                   ""S"":{include.Filter.Skip},
                   ""T"":{include.Filter.Take}
                }},
                ""P"":""{include.Path}""
            }}";

            expectedJson = expectedJson.Replace(Environment.NewLine, "").Replace(" ", "");

            var converter = new IncludeJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, include, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}
