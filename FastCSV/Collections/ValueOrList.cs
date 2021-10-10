using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV.Collections
{
    /// <summary>
    /// Represents a single value or a collection of values.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public struct ValueOrList<T> : IList<T>, IReadOnlyList<T>, IEquatable<ValueOrList<T>> where T: notnull
    {
        /// Single element list to throw the same exception message as List<T>
        private static readonly List<T> _SingleElementList = new List<T> { default(T)! };

        /// <summary>
        /// Gets an empty <see cref="ValueOrList{T}"/>.
        /// </summary>
        public static ValueOrList<T> Empty { get; } = new ValueOrList<T>(Array.Empty<T>());

        private object _value;

        /// <summary>
        /// Constructs a new <see cref="ValueOrList{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public ValueOrList(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Constructs a new <see cref="ValueOrList{T}"/> with a collection of values.
        /// </summary>
        /// <param name="values">The values to add.</param>
        public ValueOrList(IEnumerable<T> values)
        {
            _value = new List<T>(values);
        }

        /// <summary>
        /// Checks whether this instance is empty.
        /// </summary>
        public bool IsEmpty => Count == 0;

        #region IList implementation

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

            set
            {
                if (_value is List<T> list)
                {
                    list[index] = value;
                }
                else
                {
                    if (index != 0)
                    {
                        // Throws an exception
                        _SingleElementList[index] = value;
                    }

                    _value = value;
                }
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

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (_value is not List<T> list)
            {
                list = new List<T>(2);
                list.Add((T)_value);
                _value = list;
            }

            list.Add(item);
        }

        public void Insert(int index, T item)
        {
            if (_value is List<T> list)
            {
                list.Insert(index, item);
            }
            else
            {
                if (index != 0)
                {
                    // Throw exception
                    _SingleElementList.Insert(index, item);
                }

                _value = item;
            }
        }

        public bool Remove(T item)
        {
            if (_value is List<T> list)
            {
                return list.Remove(item);
            }
            else
            {
                if (EqualityComparer<T>.Default.Equals(item, (T)_value))
                {
                    _value = new List<T>();
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (_value is List<T> list)
            {
                list.RemoveAt(index);
            }
            else
            {
                if (index != 0)
                {
                    // Throw exception
                    _SingleElementList.RemoveAt(index);
                }
                else
                {
                    _value = new List<T>();
                }
            }
        }

        public void Clear()
        {
            if (_value is List<T> list)
            {
                list.Clear();
            }
            else
            {
                _value = new List<T>();
            }
        }

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

        #endregion

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueOrList<T> list && Equals(list);
        }

        public bool Equals(ValueOrList<T> other)
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

        public static implicit operator ValueOrList<T>(T value)
        {
            return new ValueOrList<T>(value);
        }

        public static implicit operator ValueOrList<T>(T[] values)
        {
            return new ValueOrList<T>(values);
        }

        public static bool operator ==(ValueOrList<T> left, ValueOrList<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueOrList<T> left, ValueOrList<T> right)
        {
            return !(left == right);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly ValueOrList<T> _valueOrList;
            private T? _current;
            private int _index;

            internal Enumerator(ValueOrList<T> valueOrList)
            {
                _valueOrList = valueOrList;
                _current = default;
                _index = 0;
            }

            public T Current => _current!;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_index < _valueOrList.Count)
                {
                    _current = _valueOrList[_index];
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
