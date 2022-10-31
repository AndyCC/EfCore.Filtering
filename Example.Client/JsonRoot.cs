using System.Text.Json.Serialization;

namespace Example.Client
{
    public class JsonRoot<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = null;
    }
}
