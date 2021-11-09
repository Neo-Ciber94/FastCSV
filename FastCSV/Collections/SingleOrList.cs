using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FastCSV.Collections
{
    /// <summary>
    /// Represents a single value or a collection of values.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public struct SingleOrList<T> : IList<T>, IReadOnlyList<T>, IEquatable<SingleOrList<T>>, IEquatable<T> where T: notnull
    {
        /// Single element list to throw the same exception message as List<T>
        private static readonly T[] s_EmptyArray = Array.Empty<T>();

        /// <summary>
        /// Gets an empty <see cref="SingleOrList{T}"/>.
        /// </summary>
        public static SingleOrList<T> Empty { get; } = new SingleOrList<T>(s_EmptyArray);

        private object _value;
        private int _count;

        /// <summary>
        /// Constructs a new <see cref="SingleOrList{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public SingleOrList(T value)
        {
            _value = value;
            _count = 1;
        }

        /// <summary>
        /// Constructs a new <see cref="SingleOrList{T}"/> with a collection of values.
        /// </summary>
        /// <param name="values">The values to add.</param>
        public SingleOrList(IEnumerable<T> values) : this(values.ToArray()) { }

        internal SingleOrList(T[] array)
        {
            _value = array;
            _count = array.Length;
        }

        /// <summary>
        /// Checks whether this instance is empty.
        /// </summary>
        public bool IsEmpty => Count == 0;

        public int Count => _count;

        /// <summary>
        /// Creates a <see cref="ReadOnlyMemory{T}"/> from this instance.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyMemory<T> AsMemory()
        {
            if (_value is not T[] array)
            {
                array = new T[1] { (T)_value };
            }

            return array.AsMemory(0, _count);
        }

        /// <summary>
        /// Creates a <see cref="ReadOnlySpan{T}"/> from this instance.
        /// </summary>
        /// <returns></returns>
        public ReadOnlySpan<T> AsSpan()
        {
            if (_value is not T[] array)
            {
                array = new T[1] { (T)_value };
            }

            return array.AsSpan(0, _count);
        }
        #region IList implementation

        public T this[int index]
        {
            get
            {
                if (_value is T[] array)
                {
                    return array[index];
                }

                if (index != 0)
                {
                    // Throws an exception
                    return s_EmptyArray[index];
                }

                return (T)_value;
            }

            set
            {
                if (_value is T[] array)
                {
                    array[index] = value;
                }
                else
                {
                    if (index != 0)
                    {
                        // Throws an exception
                        s_EmptyArray[index] = value;
                    }

                    _value = value;
                }
            }
        }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (_value is T[])
            {
                int length = ((T[])_value).Length;
                if (_count == length)
                {
                    Resize(1);
                }

                ((T[])_value)[_count] = item;
            }
            else
            {
                _value = new T[2] { (T)_value, item };
            }

            _count += 1;
        }

        private void Resize(int required)
        {
            int minCapacity = _count + required;
            int length = _value is T[] array ? array.Length : 1;

            if (minCapacity > length)
            {
                int newCapacity = Math.Max(minCapacity, _count * 2);
                T[] newArray = new T[newCapacity];

                if (_value is T[] oldArray)
                {
                    Array.Copy(oldArray, newArray, _count);
                }
                else
                {
                    newArray[0] = (T)_value;
                }

                _value = newArray;
            }
        }

        public void Insert(int index, T item)
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("this instance is empty");
            }

            if (index < 0 || index > _count)
            {
                throw new IndexOutOfRangeException($"index cannot be negative or greater than {_count} but was: {index}");
            }

            if (_value is not T[])
            {
                _value = new T[1] { (T)_value };
            }

            Resize(1);

            T[] array = (T[])_value;
            Array.Copy(array, index, array, index + 1, _count - index);
            array[index] = item;
            _count += 1;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index > _count)
            {
                throw new IndexOutOfRangeException($"index cannot be negative or greater than {_count} but was {index}");
            }

            if (_value is T[] array)
            {
                _count -= 1;

                if (index < _count)
                {
                    Array.Copy(array, index + 1, array, index, _count - index);
                }

                array[_count] = default!;
            }
            else if (index == 0)
            {
                _value = s_EmptyArray;
            }
        }

        public void Clear()
        {
            if (_value is T[] array)
            {
                Array.Clear(array, 0, _count);
            }

            _count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item)
        {
            if (_value is T[] array)
            {
                return Array.IndexOf(array, item);
            }

            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(item, (T)_value) ? 0 : -1;
        }

        public void CopyTo(T[] destination, int arrayIndex)
        {
            if (_value is T[] array)
            {
                Array.Copy(array, 0, destination, arrayIndex, _count);
            }
            else
            {
                destination[arrayIndex] = (T)_value;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public override bool Equals(object? obj)
        {
            return obj is SingleOrList<T> list && Equals(list);
        }

        public bool Equals(SingleOrList<T> other)
        {
            if (Count != other.Count)
            {
                return false;
            }

            var comparer = EqualityComparer<T>.Default;
            int count = Count;

            for(int i = 0; i < count; i++)
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

        public override string ToString()
        {
            return string.Join(", ", this);
        }

        public bool Equals(T? other)
        {
            if (other == null)
            {
                return false;
            }

            if (_count > 1)
            {
                return false;
            }

            return EqualityComparer<T>.Default.Equals((T)_value, other);
        }

        public static implicit operator SingleOrList<T>(T value)
        {
            return new SingleOrList<T>(value);
        }

        public static implicit operator SingleOrList<T>(T[] values)
        {
            return new SingleOrList<T>(values);
        }

        public static bool operator ==(SingleOrList<T> left, SingleOrList<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SingleOrList<T> left, SingleOrList<T> right)
        {
            return !(left == right);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly SingleOrList<T> _singleOrList;
            private T? _current;
            private int _index;

            internal Enumerator(SingleOrList<T> valueOrList)
            {
                _singleOrList = valueOrList;
                _current = default;
                _index = -1;
            }

            public T Current => _current!;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                int next = _index + 1;
                if (next < _singleOrList.Count)
                {
                    _current = _singleOrList[next];
                    _index = next;
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