using EfCore.Filtering.Client;
using EfCore.Filtering.Client.Serialization;
using NUnit.Framework;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    public class IncludeJsonConverter_ReadTests
    {
        [Test]
        public void ShortForm_ItReadsPath()
        {
            const string expectedPath = "Property.Path";
            const string json = $"{{\"P\":\"{expectedPath}\"}}";

            var converter = new IncludeJsonConverter();
            var jsonReader = json.GetJsonReader();
            var include = converter.Read(ref jsonReader, typeof(Include), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(include);
            Assert.That(include.Path, Is.EqualTo(expectedPath));
            Assert.IsNull(include.Filter);
        }

        [Test]
        public void LongForm_ItReadsPath()
        {
            const string expectedPath = "Property.Path";
            const string json = $"{{\"path\":\"{expectedPath}\"}}";

            var converter = new IncludeJsonConverter();
            var jsonReader = json.GetJsonReader();
            var include = converter.Read(ref jsonReader, typeof(Include), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(include);
            Assert.That(include.Path, Is.EqualTo(expectedPath));
            Assert.IsNull(include.Filter);
        }

        [Test]
        public void ShortForm_ItReadsFilter()
        {
            const string expectedPath = "Property.Path";
            const int expectedTake = 1;
            const int expectedSkip = 2;

            string json = $@"{{
                                ""p"":""{expectedPath}"",
                                ""f"": {{
                                           ""s"":{expectedSkip}, 
                                           ""t"":{expectedTake}
                                       }} 
                                }}";

            var converter = new IncludeJsonConverter();
            var jsonReader = json.GetJsonReader();
            var include = converter.Read(ref jsonReader, typeof(Include), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(include);
            Assert.That(include.Path, Is.EqualTo(expectedPath));

            Assert.IsNotNull(include.Filter);
            Assert.That(include.Filter.Take, Is.EqualTo(expectedTake));
            Assert.That(include.Filter.Skip, Is.EqualTo(expectedSkip));
        }

        [Test]
        public void LongForm_ItReadsFilter()
        {
            const string expectedPath = "Property.Path";
            const int expectedTake = 1;
            const int expectedSkip = 2;

            string json = $@"{{
                                ""path"":""{expectedPath}"",
                                ""filter"": {{
                                           ""skip"":{expectedSkip}, 
                                           ""take"":{expectedTake}
                                       }} 
                                }}";

            var converter = new IncludeJsonConverter();
            var jsonReader = json.GetJsonReader();
            var include = converter.Read(ref jsonReader, typeof(Include), SerializationTestHelpers.SerializeOptions);

            Assert.IsNotNull(include);
            Assert.That(include.Path, Is.EqualTo(expectedPath));

            Assert.IsNotNull(include.Filter);
            Assert.That(include.Filter.Take, Is.EqualTo(expectedTake));
            Assert.That(include.Filter.Skip, Is.EqualTo(expectedSkip));
        }
    }
}
