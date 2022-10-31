using System;
using System.Collections.Generic;

namespace EfCore.Filtering.Client.Serialization.Common
{
    public class ReaderOptions<T>
    {
        public ReaderOptions(IDictionary<string, string> shortToLongForPropertyNameMap = null,
            IDictionary<string, InterceptPropertyRead<T>> propertyReadInterceptors = null,
            IDictionary<string, Func<string, string>> stringPropertyTranslators = null)
        {
            ShortToLongForPropertyNameMap = shortToLongForPropertyNameMap ?? new Dictionary<string, string>();
            PropertyReadInterceptors = propertyReadInterceptors ?? new Dictionary<string, InterceptPropertyRead<T>>();
            StringPropertyTranslators = stringPropertyTranslators ?? new Dictionary<string, Func<string, string>>();
        }

        public IDictionary<string, string> ShortToLongForPropertyNameMap { get; private set; }
        public IDictionary<string, InterceptPropertyRead<T>> PropertyReadInterceptors { get; private set; }
        public IDictionary<string, Func<string, string>> StringPropertyTranslators { get; private set; }
    };
}
