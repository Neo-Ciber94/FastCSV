using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FastCSV.Utils
{
    /// <summary>
    /// Represents a dictionary of objects, this can be used for singletons of untyped objects.
    /// </summary>
    public class ObjectDictionary : IReadOnlyDictionary<Type, object>
    {
        private readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();

        /// <summary>
        /// Gets the number of objects stored.
        /// </summary>
        public int Count => instances.Count;

        /// <summary>
        /// Gets an object related to the given type.
        /// </summary>
        /// <param name="type">Type to get.</param>
        /// <returns>The type related to the given type.</returns>
        public object this[Type type] => instances[type];

        /// <summary>
        /// Adds an type-object pair.
        /// </summary>
        /// <param name="type">Type to relate the object to.</param>
        /// <param name="value">Value to add.</param>
        public void Add(Type type, object value)
        {
            instances.Add(type, value);
        }

        /// <summary>
        /// Removes the object related to the given type.
        /// </summary>
        /// <param name="type">Type to delete the object.</param>
        /// <returns><c>true</c> if the object is deleted.</returns>
        public bool Remove(Type type)
        {
            return instances.Remove(type);
        }

        /// <summary>
        /// Attempts to get a value related to the given type.
        /// </summary>
        /// <param name="type">The type related to the object.</param>
        /// <param name="value">The resulting value.</param>
        /// <returns><c>true</c> if the value was found.</returns>
        public bool TryGetValue(Type type, [MaybeNullWhen(false)] out object value)
        {
            return instances.TryGetValue(type, out value);
        }

        /// <summary>
        /// Check whether there is a object related to the given type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns><c>true</c> if there is a value related to the given type.</returns>
        public bool ContainsKey(Type type)
        {
            return instances.ContainsKey(type);
        }

        IEnumerable<Type> IReadOnlyDictionary<Type, object>.Keys => instances.Keys;

        IEnumerable<object> IReadOnlyDictionary<Type, object>.Values => instances.Values;

        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
        {
            return instances.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<Type, object>> IEnumerable<KeyValuePair<Type, object>>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
