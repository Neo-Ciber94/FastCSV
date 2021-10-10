using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using FastCSV.Utils;

namespace FastCSV.Collections
{
    /// <summary>
    /// Represents a readonly view to an array.
    /// 
    /// <para>
    /// This struct do not ensure the inner array is not mutated.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public readonly struct ReadOnlyArray<T> : IEnumerable<T>, IReadOnlyList<T>, IEquatable<ReadOnlyArray<T>>
    {
        // An empty array to throw array-like exceptions
        private static readonly T[] s_EmptyArray = Array.Empty<T>();

        /// <summary>
        /// An empty <see cref="ReadOnlyArray{T}"/>.
        /// </summary>
        public static ReadOnlyArray<T> Empty => new ReadOnlyArray<T>();

        internal readonly T[]? _source;

        /// <summary>
        /// Constructs a new <see cref="ReadOnlyArray{T}"/>.
        /// </summary>
        /// <param name="array">The source array.</param>
        public ReadOnlyArray(T[] array)
        {
            _source = array;
        }

        /// <summary>
        /// Gets the element at the given index of the array.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                if (_source == null)
                {
                    return s_EmptyArray[index];
                }

                return _source[index];
            }
        }

        /// <summary>
        /// Gets the element at the given index of the array.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[Index index]
        {
            get
            {
                if (_source == null)
                {
                    return s_EmptyArray[index];
                }

                return _source[index];
            }
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlySpan{T}"/> to the elements in the given range.
        /// </summary>
        /// <param name="range">The range to take the elements from.</param>
        /// <returns>A readonly span view to the elements.</returns>
        public ReadOnlySpan<T> this[Range range]
        {
            get
            {
                if (_source == null)
                {
                    return s_EmptyArray[range];
                }

                return _source[range];
            }
        }

        /// <summary>
        /// Gets the number of elements in this <see cref="ReadOnlyArray{T}"/>.
        /// </summary>
        public int Count
        {
            get
            {
                if (_source == null)
                {
                    return 0;
                }

                return _source.Length;
            }
        }

        /// <summary>
        /// Checks if this <see cref="ReadOnlyArray{T}"/> have no elements.
        /// </summary>
        public bool IsEmpty => _source == null || _source.Length == 0;

        /// <summary>
        /// Gets an <see cref="ReadOnlyMemory{T}"/> view to the elements of this array.
        /// </summary>
        /// <returns>A view to the elements.</returns>
        public ReadOnlyMemory<T> AsMemory()
        {
            if (_source == null)
            {
                return ReadOnlyMemory<T>.Empty;
            }

            return _source;
        }

        /// <summary>
        /// Gets an <see cref="ReadOnlyMemory{T}"/> view to the elements of this array form the given index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <returns>A view to the elements.</returns>
        public ReadOnlyMemory<T> AsMemory(int startIndex)
        {
            if (_source == null)
            {
                return ReadOnlyMemory<T>.Empty;
            }

            return _source.AsMemory(startIndex);
        }

        /// <summary>
        /// Gets an <see cref="ReadOnlyMemory{T}"/> view to the elements of this array form the given index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <returns>A view to the elements.</returns>
        public ReadOnlyMemory<T> AsMemory(Index startIndex)
        {
            if (_source == null)
            {
                return ReadOnlyMemory<T>.Empty;
            }

            return _source.AsMemory(startIndex);
        }

        /// <summary>
        /// Gets an <see cref="ReadOnlyMemory{T}"/> view to the elements of this array form the given index until the given number of elements.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of elements</param>
        /// <returns>A view to the elements.</returns>
        public ReadOnlyMemory<T> AsMemory(int startIndex, int count)
        {
            if (_source == null)
            {
                return ReadOnlyMemory<T>.Empty;
            }

            return _source.AsMemory(startIndex, count);
        }

        /// <summary>
        /// Gets an <see cref="ReadOnlyMemory{T}"/> view to the elements of this array form the given range.
        /// </summary>
        /// <param name="range">The range of elements.</param>
        /// <returns>A view to the elements.</returns>
        public ReadOnlyMemory<T> AsMemory(Range range)
        {
            if (_source == null)
            {
                return ReadOnlyMemory<T>.Empty;
            }

            return _source.AsMemory(range);
        }


        /// <summary>
        /// Gets an <see cref="ReadOnlySpan{T}"/> view to the elements of this array.
        /// </summary>
        /// <returns>A view to the elements.</returns>
        public ReadOnlySpan<T> AsSpan()
        {
            if (_source == null)
            {
                return ReadOnlySpan<T>.Empty;
            }

            return _source;
        }

        /// <summary>
        /// Gets an <see cref="ReadOnlySpan{T}"/> view to the elements of this array form the given index until the given number of elements.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <returns>A view to the elements.</returns>
        public ReadOnlySpan<T> AsSpan(int startIndex)
        {
            if (_source == null)
            {
                return ReadOnlySpan<T>.Empty;
            }

            return _source.AsSpan(startIndex);
        }

        /// <summary>
        /// Gets an <see cref="ReadOnlySpan{T}"/> view to the elements of this array form the given index until the given number of elements.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of elements</param>
        /// <returns>A view to the elements.</returns>
        public ReadOnlySpan<T> AsSpan(int startIndex, int count)
        {
            if (_source == null)
            {
                return ReadOnlySpan<T>.Empty;
            }

            return _source.AsSpan(startIndex, count);
        }

        /// <summary>
        /// Gets an <see cref="ReadOnlySpan{T}"/> view to the elements of this array form the given index until the given number of elements.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <returns>A view to the elements.</returns>
        public ReadOnlySpan<T> AsSpan(Index startIndex)
        {
            if (_source == null)
            {
                return ReadOnlySpan<T>.Empty;
            }

            return _source.AsSpan(startIndex);
        }

        /// <summary>
        /// Gets an <see cref="ReadOnlySpan{T}"/> view to the elements of this array form the given range.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of elements</param>
        /// <returns>A view to the elements.</returns>
        public ReadOnlySpan<T> AsSpan(Range range)
        {
            if (_source == null)
            {
                return ReadOnlySpan<T>.Empty;
            }

            return _source.AsSpan(range);
        }


        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        public int IndexOf(T value)
        {
            if (_source == null)
            {
                return -1;
            }

            return Array.IndexOf(_source, value);
        }

        /// <summary>
        /// Checks whether this array contains the specified element.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns><c>true</c> if found otherwise false.</returns>
        public bool Contains(T value)
        {
            if (_source == null)
            {
                return false;
            }

            return Array.IndexOf(_source, value) >= 0;
        }

        /// <summary>
        /// Creates a copy of the elements of this <see cref="ReadOnlyArray{T}"/>.
        /// </summary>
        /// <returns>A copy of the elements of this array.</returns>
        public T[] ToArray()
        {
            if (_source == null)
            {
                return s_EmptyArray;
            }

            T[] array = new T[_source.Length];
            Array.Copy(_source, array, _source.Length);
            return array;
        }

        /// <summary>
        /// Gets an enumerator over the elements of this array.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_source?? s_EmptyArray);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override bool Equals(object? obj)
        {
            return obj is ReadOnlyArray<T> array && Equals(array);
        }

        public bool Equals(ReadOnlyArray<T> other)
        {
            T[] selfArray = _source ?? s_EmptyArray;
            T[] otherArray = other._source ?? s_EmptyArray;

            if (selfArray.Length != otherArray.Length)
            {
                return false;
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for(int i = 0; i < selfArray.Length; i++)
            {
                T x = selfArray[i];
                T y = otherArray[i];

                if (!comparer.Equals(x, y))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_source);
        }

        public override string ToString()
        {
            if (_source == null)
            {
                return "[]";
            }

            var stringBuilder = StringBuilderCache.Acquire();
            stringBuilder.Append('[');

            for (int i = 0; i < _source.Length; i++)
            {
                string s = _source[i]?.ToString() ?? "null";
                stringBuilder.Append(s);

                if (i < _source.Length - 1)
                {
                    stringBuilder.Append(',');
                    stringBuilder.Append(' ');
                }
            }

            stringBuilder.Append(']');
            return StringBuilderCache.ToStringAndRelease(ref stringBuilder!);
        }

        public static implicit operator ReadOnlyArray<T>(T[] array)
        {
            return new ReadOnlyArray<T>(array);
        }

        public static bool operator ==(ReadOnlyArray<T> left, ReadOnlyArray<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReadOnlyArray<T> left, ReadOnlyArray<T> right)
        {
            return !(left == right);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly T[] _source;
            private int index;

            public Enumerator(T[] array)
            {
                _source = array;
                index = -1;
            }

            public T Current => _source[index];

            public bool MoveNext()
            {
                if (_source == null)
                {
                    return false;
                }

                int next = index + 1;

                if (next < _source.Length)
                {
                    index = next;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                index = 0;
            }

            object IEnumerator.Current => Current!;

            void IDisposable.Dispose() { }
        }
    }

    /// <summary>
    /// Utility class for <see cref="ReadOnlyArray{T}"/>.
    /// </summary>
    public static class ReadOnlyArray
    {
        /// <summary>
        /// Creates a <see cref="ReadOnlyArray{T}"/> from the given elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="elements">Elements to use.</param>
        /// <returns>A readonly array with the given elements.</returns>
        public static ReadOnlyArray<T> Create<T>(params T[] elements)
        {
            return new ReadOnlyArray<T>(elements);
        }

        /// <summary>
        /// Creates an <see cref="ImmutableArray{T}"/> from this instance.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="array">An readonly array.</param>
        /// <returns>An immutable array from this instance</returns>
        public static ImmutableArray<T> ToImmutable<T>(this ReadOnlyArray<T> array)
        {
            return ImmutableArray.Create(array._source);
        }
    }
}
