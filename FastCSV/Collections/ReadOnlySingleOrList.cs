using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FastCSV.Collections
{
    /// <summary>
    /// Represents a single value or a collection of values.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public readonly struct ReadOnlySingleOrList<T> : IReadOnlyList<T>, IEquatable<ReadOnlySingleOrList<T>> where T : notnull
    {
        /// Single element list to throw the same exception message as List<T>
        private static readonly List<T> _SingleElementList = new List<T> { default(T)! };

        /// <summary>
        /// Gets an empty <see cref="ReadOnlySingleOrList{T}"/>.
        /// </summary>
        public static ReadOnlySingleOrList<T> Empty { get; } = new ReadOnlySingleOrList<T>(Array.Empty<T>());

        private readonly object _value;

        /// <summary>
        /// Constructs a new <see cref="ReadOnlySingleOrList{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public ReadOnlySingleOrList(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Constructs a new <see cref="ReadOnlySingleOrList{T}"/> with a collection of values.
        /// </summary>
        /// <param name="values">The values to add.</param>
        public ReadOnlySingleOrList(IEnumerable<T> values)
        {
            _value = new List<T>(values);
        }

        /// <summary>
        /// Checks whether this instance is empty.
        /// </summary>
        public bool IsEmpty => Count == 0;

        public T this[int index]
        {
            get
            {
                if (_value is List<T> list)
                {
                    return list[index];
                }

                if (index != 0)
                {
                    // Throws an exception
                    return _SingleElementList[index];
                }

                return (T)_value;
            }
        }

        public int Count
        {
            get
            {
                if (_value == null)
                {
                    return 0;
                }

                return _value is List<T> list ? list.Count : 1;
            }
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="ReadOnlySingleOrList{T}"/>.
        /// </summary>
        /// <param name="item">The item to locate..</param>
        /// <returns><c>true</c> if the element is found.</returns>
        public bool Contains(T item)
        {
            if (_value is List<T> list)
            {
                return list.Contains(item);
            }
            else
            {
                return EqualityComparer<T>.Default.Equals(item, (T)_value);
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the entire <see cref="ReadOnlySingleOrList{T}"/>.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The index of the item or -1 if not found.</returns>
        public int IndexOf(T item)
        {
            if (_value is List<T> list)
            {
                return list.IndexOf(item);
            }
            else
            {
                if (EqualityComparer<T>.Default.Equals(item, (T)_value))
                {
                    return 0;
                }

                return -1;
            }
        }

        /// <summary>
        /// Copies the values in this <see cref="ReadOnlySingleOrList{T}"/> to the array.
        /// </summary>
        /// <param name="array">The array to copies the values.</param>
        /// <param name="arrayIndex">The array index where start to copies the elements.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_value is List<T> list)
            {
                list.CopyTo(array, arrayIndex);
            }
            else
            {
                array[arrayIndex] = (T)_value;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public override bool Equals(object? obj)
        {
            return obj is ReadOnlySingleOrList<T> list && Equals(list);
        }

        public bool Equals(ReadOnlySingleOrList<T> other)
        {
            if (Count != other.Count)
            {
                return false;
            }

            var comparer = EqualityComparer<T>.Default;
            int count = Count;

            for (int i = 0; i < count; i++)
            {
                if (!comparer.Equals(this[i], other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public static implicit operator ReadOnlySingleOrList<T>(T value)
        {
            return new ReadOnlySingleOrList<T>(value);
        }

        public static implicit operator ReadOnlySingleOrList<T>(T[] values)
        {
            return new ReadOnlySingleOrList<T>(values);
        }

        public static implicit operator ReadOnlySingleOrList<T>(SingleOrList<T> values)
        {
            return Unsafe.As<SingleOrList<T>, ReadOnlySingleOrList<T>>(ref values);
        }

        public static bool operator ==(ReadOnlySingleOrList<T> left, ReadOnlySingleOrList<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReadOnlySingleOrList<T> left, ReadOnlySingleOrList<T> right)
        {
            return !(left == right);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly ReadOnlySingleOrList<T> _ReadOnlyValueOrList;
            private T? _current;
            private int _index;

            internal Enumerator(ReadOnlySingleOrList<T> ReadOnlyValueOrList)
            {
                _ReadOnlyValueOrList = ReadOnlyValueOrList;
                _current = default;
                _index = 0;
            }

            public T Current => _current!;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_index < _ReadOnlyValueOrList.Count)
                {
                    _current = _ReadOnlyValueOrList[_index];
                    _index += 1;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _index = 0;
            }

            public void Dispose()
            {
            }
        }
    }
}
