using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace FastCSV.Utils
{
    /// <summary>
    /// Represents a value with an index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.IEquatable{FastCSV.Utils.IndexedValue{T}}" />
    [Serializable]
    public readonly struct IndexedValue<T> : IEquatable<IndexedValue<T>>
    {
        public IndexedValue(T value, int index)
        {
            Value = value;
            Index = index;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out T value, out int index)
        {
            value = Value;
            index = Index;
        }

        public override bool Equals(object? obj)
        {
            return obj is IndexedValue<T> value && Equals(value);
        }

        public bool Equals(IndexedValue<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value) &&
                   Index == other.Index;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Index);
        }

        public static bool operator ==(IndexedValue<T> left, IndexedValue<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IndexedValue<T> left, IndexedValue<T> right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{{{nameof(Value)}={Value}, {nameof(Index)}={Index}}}";
        }
    }

    public static class IndexedValueExtensions
    {
        /// <summary>
        /// Gets an enumerable where each value have an index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>An enumerable where each value have an index.</returns>
        public static IEnumerable<IndexedValue<T>> WithIndex<T>(this IEnumerable<T> enumerable, int startIndex = 0)
        {
            foreach(T element in enumerable)
            {
                yield return new IndexedValue<T>(element, startIndex++);
            }
        }
    }
}
