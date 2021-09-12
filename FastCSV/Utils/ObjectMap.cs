using System;
using System.Collections.Generic;

namespace FastCSV.Utils
{
    /// <summary>
    /// Represents a collection of key-value of <see cref="Type"/> and <see cref="object"/>.
    /// </summary>
    public class ObjectMap
    {
        private readonly Dictionary<Type, object> items = new Dictionary<Type, object>();

        /// <summary>
        /// Adds <see cref="Type"/> and <see cref="object"/> pair.
        /// </summary>
        /// <param name="type">The type related to the value.</param>
        /// <param name="value">The value.</param>
        public void Add(Type type, object value)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            items.Add(type, value);
        }

        /// <summary>
        /// Gets the object related with the given type or null if not found.
        /// </summary>
        /// <param name="type">The type related to the object.</param>
        /// <returns>The object related to the type, or null if not found.</returns>
        public object? Get(Type type)
        {
            if (items.TryGetValue(type, out object? value))
            {
                return value;
            }

            return value;
        }

        /// <summary>
        /// Attemps to get the object related with the given type.
        /// </summary>
        /// <param name="type">The type related to the object.</param>
        /// <param name="value">The result value.</param>
        /// <returns><c>true</c> if the value was found.</returns>
        public bool TryGet(Type type, out object? value)
        {
            return items.TryGetValue(type, out value);
        }

        /// <summary>
        /// Checks if this instance contains a object related with the given type.
        /// </summary>
        /// <param name="type">The type related of the object.</param>
        /// <returns><c>true</c> if a value was found.</returns>
        public bool Contains(Type type)
        {
            return items.ContainsKey(type);
        }
    }
}
