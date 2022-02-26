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
    }
}
