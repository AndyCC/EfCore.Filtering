using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EfCore.Filtering.Client.Serialization.Common
{
    public class PropertyMap
    {
        public PropertyMap(IDictionary<string, string> initalValues)
        {
            _shortNameToLongName = new ReadOnlyDictionary<string, string> (initalValues);

            var reversedDictionary = new Dictionary<string, string> ();

            foreach (var kvp in initalValues)
                reversedDictionary.Add(kvp.Value, kvp.Key);

            _longNameToShortName = new ReadOnlyDictionary<string, string>(reversedDictionary);
        }

        private IDictionary<string, string> _shortNameToLongName;
        private IDictionary<string, string> _longNameToShortName;

        public IDictionary<string, string> ShortNameToLongName { get { return _shortNameToLongName; } } 
        public IDictionary<string, string> LongNameToShortName { get { return _longNameToShortName; } }
    }
}
