using System.Text.Json;

namespace EfCore.Filtering.Client.Serialization.Common
{
    public delegate void InterceptPropertyWrite<T>(T target, ref Utf8JsonWriter writer, JsonSerializerOptions options);
}
