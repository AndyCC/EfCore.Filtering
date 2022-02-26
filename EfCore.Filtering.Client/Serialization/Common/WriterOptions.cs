using System;
using System.Collections.Generic;

namespace EfCore.Filtering.Client.Serialization.Common
{
    public class WriterOptions<T>
    {
        public WriterOptions(IDictionary<string, string> longToShortPropertyNameMap = null,
            IDictionary<string, Func<string, string>> stringPropertyTranslators = null)
        {
            LongToShortForPropertyNameMap = longToShortPropertyNameMap ?? new Dictionary<string, string>();
            StringPropertyTranslators = stringPropertyTranslators ?? new Dictionary<string, Func<string, string>>();
        }

        public IDictionary<string, string> LongToShortForPropertyNameMap { get; private set; }
        public IDictionary<string, Func<string, string>> StringPropertyTranslators { get; private set; }
    };
}
