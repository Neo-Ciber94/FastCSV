using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FastCSV.Collections
{
    internal struct ArrayBuilder<T> : IEnumerable<T>, IDisposable
    {
        private T[]? _arrayFromPool;
        private int _count;

        public ArrayBuilder(int initialCapacity)
        {
            _arrayFromPool = ArrayPool<T>.Shared.Rent(initialCapacity);
            _count = 0;
        }

        public int Count => _count;

        public int Capacity => _arrayFromPool?.Length ?? 0;

        public bool IsEmpty => _count == 0;

        public Span<T> Span
        {
            get
            {
                if (_arrayFromPool == null)
                {
                    return Span<T>.Empty;
                }

                return _arrayFromPool.AsSpan(0, _count);
            }
        }

        public void Add(T value)
        {
            ThrowIfDisposed();

            if (_arrayFromPool == null)
            {
                return;
            }

            if (_count == _arrayFromPool.Length)
            {
                ResizeIfNeeded(1);
            }

            _arrayFromPool[_count++] = value;
        }

        public void AddRange(ReadOnlySpan<T> values)
        {
            ResizeIfNeeded(values.Length);

            values.CopyTo(_arrayFromPool.AsSpan(_count));
            _count += values.Length;
        }

        public bool Remove(T value)
        {
            ThrowIfDisposed();

            var comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < _count; i++)
            {
                if (comparer.Equals(_arrayFromPool![i], value))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            ThrowIfDisposed();

            if (index < 0 || index >= _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index cannot be negative or greater than count but was {_count}");
            }

            _count -= 1;

            if (index < _count)
            {
                T[] array = _arrayFromPool!;
                Array.Copy(array, index + 1, array, index, _count - index);
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _arrayFromPool![_count] = default!;
            }
        }

        public T RemoveLast()
        {
            ThrowIfDisposed();

            if (IsEmpty)
            {
                throw new InvalidOperationException("ArrayBuilder is empty");
            }

            T last = _arrayFromPool![_count - 1];
            RemoveAt(_count - 1);
            return last;
        }

        public void Clear()
        {
            ThrowIfDisposed();

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_arrayFromPool!, 0, _count);
            }

            _count = 0;
        }

        public T[] ToArray()
        {
            ThrowIfDisposed();

            if (_arrayFromPool == null)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[_count];
            Array.Copy(_arrayFromPool, array, _count);
            return array;
        }

        private void ResizeIfNeeded(int required)
        {
            if (_arrayFromPool == null)
            {
                return;
            }

            int minRequired = _count + required;

            if (minRequired > _arrayFromPool.Length)
            {
                int count = _count == 0 ? 4 : _count * 2;
                int newCapacity = Math.Min(count, minRequired);

                T[] newArray = ArrayPool<T>.Shared.Rent(newCapacity);
                Array.Copy(_arrayFromPool, newArray, _count);
                ArrayPool<T>.Shared.Return(_arrayFromPool);
                _arrayFromPool = newArray;
            }
        }

        public void Dispose()
        {
            if (_arrayFromPool != null)
            {
                ArrayPool<T>.Shared.Return(_arrayFromPool);
                _arrayFromPool = null;
                _count = 0;
            }
        }

        [Conditional("DEBUG")]
        private void ThrowIfDisposed()
        {
            if (_arrayFromPool == null)
            {
                throw new InvalidOperationException($"{GetType()} is uninitialized or disposed");
            }
        }

        public ArrayEnumerator<T> GetEnumerator()
        {
            ThrowIfDisposed();
            return new ArrayEnumerator<T>(_arrayFromPool!, _count);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
