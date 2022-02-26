using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using NUnit.Framework;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class OrderByJsonConverter_ReadTests
    {
        [Test]
        [TestCase("+")]
        [TestCase("\u002B")]
        public void ShortForm_ItReadsOrderingAsc(string plus)
        {
            const string expectedPath = "Property.Path";
            var json = $"\"{plus}{expectedPath}\"";

            var converter = new OrderByJsonConverter();
            var jsonReader = json.GetJsonReader();
            var orderBy = converter.Read(ref jsonReader, typeof(OrderBy), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(orderBy);
            Assert.That(orderBy.Order, Is.EqualTo(Ordering.ASC));
            Assert.That(orderBy.Path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void ShortForm_ItReadsOrderingDesc()
        {
            const string expectedPath = "Property.Path";
            var json = $"\"-{expectedPath}\"";

            var converter = new OrderByJsonConverter();
            var jsonReader = json.GetJsonReader();
            var orderBy = converter.Read(ref jsonReader, typeof(OrderBy), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(orderBy);
            Assert.That(orderBy.Order, Is.EqualTo(Ordering.DESC));
            Assert.That(orderBy.Path, Is.EqualTo(expectedPath));
        }

        [Test]
        [TestCase("asc")]
        [TestCase("desc")]
        public void LongForm_ItReadsOrdering(string expectedOrder)
        {
            const string expectedPath = "Property.Path";
            var json = $"{{\"Path\":\"{expectedPath}\",\"Order\":\"{expectedOrder}\"}}";

            var converter = new OrderByJsonConverter();
            var jsonReader = json.GetJsonReader();
            var orderBy = converter.Read(ref jsonReader, typeof(OrderBy), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(orderBy);
            Assert.That(orderBy.Order, Is.EqualTo(expectedOrder));
            Assert.That(orderBy.Path, Is.EqualTo(expectedPath));
        }
    }
}
