using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using NUnit.Framework;
using System.IO;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class OrderByJsonConverter_WriteTests
    {
        [Test]
        public void ItWritesAscendingValue()
        {
            var orderBy = new OrderBy
            {
                Order = Ordering.ASC,
                Path = "My.Property.Path"
            };

            var converter = new OrderByJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);

            converter.Write(writer, orderBy, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo($"\"\\u002B{orderBy.Path}\""));
        }

        [Test]
        public void ItWritesDescendingValue()
        {
            var orderBy = new OrderBy
            {
                Order = Ordering.DESC,
                Path = "My.Property.Path"
            };

            var converter = new OrderByJsonConverter();

            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);


            converter.Write(writer, orderBy, SerializationTestHelpers.SerializeOptions);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            Assert.That(json, Is.EqualTo($"\"-{orderBy.Path}\""));

        }
    }
}
