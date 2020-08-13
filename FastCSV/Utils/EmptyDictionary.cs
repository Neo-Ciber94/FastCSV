using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FastCSV.Utils
{
    internal static class EmptyDictionary<TKey, TValue> where TKey : notnull
    {
        public static readonly IReadOnlyDictionary<TKey, TValue> Value = new EmptyDictionaryImpl();

        class EmptyDictionaryImpl : IReadOnlyDictionary<TKey, TValue>
        {
            public TValue this[TKey key] => throw new KeyNotFoundException(key.ToString());

            public IEnumerable<TKey> Keys => Array.Empty<TKey>();

            public IEnumerable<TValue> Values => Array.Empty<TValue>();

            public int Count => 0;

            public bool ContainsKey(TKey key) => false;

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
            {
                value = default;
                return false;
            }
        }
    }
}
