using System.Text.Json;

namespace EfCore.Filtering.Client.Serialization.Common
{
    public delegate void InterceptPropertyRead<T>(T target, ref Utf8JsonReader reader, JsonSerializerOptions options);
}
