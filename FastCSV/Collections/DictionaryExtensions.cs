using System.Collections.Generic;

namespace FastCSV.Collections
{
    internal static class DictionaryExtensions
    {
        public static TValue? GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            if (dictionary.TryGetValue(key, out TValue? value))
            {
                return value;
            }

            return null;
        }
    }
}
