using System.Collections;
using System.Linq;
using System.Text.Json;

namespace EfCore.Filtering.Client.Serialization.Common
{
    public static class Writer
    {
        public static void Write<T>(Utf8JsonWriter writer, JsonSerializerOptions jsonOptions, WriterOptions<T> writeOptions, T model)
        {
            if(model == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            foreach(var property in model.GetType().GetProperties().OrderBy(x => x.Name))
            {
                if (!writeOptions.LongToShortForPropertyNameMap.ContainsKey(property.Name))
                    continue;

                var propertyValue = property.GetValue(model, null);

                if (propertyValue == null || IsEmptyList(propertyValue))
                    continue;

                var shortName = writeOptions.LongToShortForPropertyNameMap[property.Name];
                writer.WritePropertyName(shortName);

                if (TryWriteString(writer, writeOptions, property.Name, propertyValue as string))
                    continue;

                JsonSerializer.Serialize(writer, propertyValue, propertyValue.GetType(), jsonOptions);                
            }

            writer.WriteEndObject();
        }

        private static bool IsEmptyList(object propertyValue)
        {
            var asEnumerable = propertyValue as ICollection;

            return asEnumerable != null && asEnumerable.Count == 0; 
        }

        private static bool TryWriteString<T>(Utf8JsonWriter writer, WriterOptions<T> writeOptions, string propertyName, string propertyValue)
        {
            if (propertyValue == null)
                return false;

            if (!writeOptions.StringPropertyTranslators.ContainsKey(propertyName))
                return false;

            writer.WriteStringValue(writeOptions.StringPropertyTranslators[propertyName](propertyValue));
            return true;
        }
    }
}
