using EfCore.Filtering.Client.Serialization;
using System.Text;
using System.Text.Json;

namespace Tests.EfCore.Filtering.Client.Serialization
{
    internal static class SerializationTestHelpers
    {
        public static Utf8JsonReader GetJsonReader(this string json)
        {
            return new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
        }

        public static JsonSerializerOptions SerializeOptions
        {
            get
            {
                var options = new JsonSerializerOptions();
                options.AddFilterConvertors();
                return options;
            }
        }
    }
}
