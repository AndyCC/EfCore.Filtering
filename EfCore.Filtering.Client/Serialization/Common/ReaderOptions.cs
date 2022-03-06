using System;
using System.Collections.Generic;

namespace EfCore.Filtering.Client.Serialization.Common
{
    public class ReaderOptions<T>
    {
        public ReaderOptions(Dictionary<string, string> shortToLongForPropertyNameMap = null,
            Dictionary<string, InterceptPropertyRead<T>> propertyReadInterceptors = null,
            Dictionary<string, Func<string, string>> stringPropertyTranslators = null)
        {
            ShortToLongForPropertyNameMap = shortToLongForPropertyNameMap ?? new();
            PropertyReadInterceptors = propertyReadInterceptors ?? new();
            StringPropertyTranslators = stringPropertyTranslators ?? new();
        }

        public Dictionary<string, string> ShortToLongForPropertyNameMap { get; private set; }
        public Dictionary<string, InterceptPropertyRead<T>> PropertyReadInterceptors { get; private set; }
        public Dictionary<string, Func<string, string>> StringPropertyTranslators { get; private set; }
    };
}
