using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EfCore.Filtering.Client.Serialization
{
    public static class JsonSerializerOptionsExtensions
    {
        public static void AddFilterConvertors(this JsonSerializerOptions options)
        {
            options.AddConvertor(new FilterJsonConverter());  
            options.AddConvertor(new RuleJsonConverter());
            options.AddConvertor(new RuleSetJsonConverter());
            options.AddConvertor(new OrderByJsonConverter());
            options.AddConvertor(new IncludeJsonConverter());
        }
        private static void AddConvertor(this JsonSerializerOptions options, JsonConverter converter)
        {
            if (options.Converters.All(x => x.GetType() != converter.GetType()))
                options.Converters.Add(converter);
        }
    }
}
