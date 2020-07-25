using System;
using System.Collections.Generic;

namespace FastCSV.Utils
{
    [Serializable]
    public struct IndexedValue<T> : IEquatable<IndexedValue<T>>
    {
        public IndexedValue(T value, int index)
        {
            Value = value;
            Index = index;
        }

        public T Value { get; }
        public int Index { get; }

        public IndexedValue<T> WithValue(T value)
        {
            return new IndexedValue<T>(value, this.Index);
        }

        public IndexedValue<T> WithIndex(int index)
        {
            return new IndexedValue<T>(this.Value, index);
        }

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

        public override string ToString()
        {
            return $"IndexedValue(Value={Value},Index={Index})";
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
    }

    public static class IndexedValueExtensions
    {
        public static IEnumerable<IndexedValue<T>> WithIndex<T>(this IEnumerable<T> enumerable)
        {
            int index = 0;

            foreach(var e in enumerable)
            {
                yield return new IndexedValue<T>(e, index);
            }
        }
    }
}
