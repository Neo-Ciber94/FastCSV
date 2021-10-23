using System;
using System.Buffers;

namespace FastCSV.Collections
{
    internal struct ArrayBuilder<T> : IDisposable
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
            if (_arrayFromPool == null)
            {
                return;
            }

            if (_count == _arrayFromPool.Length)
            {
                Resize();
            }

            _arrayFromPool[_count++] = value;
        }

        public T[] ToArray()
        {
            if (_arrayFromPool == null)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[_count];
            Array.Copy(_arrayFromPool, array, _count);
            return array;
        }

        private void Resize()
        {
            if (_arrayFromPool == null)
            {
                return;
            }

            int newCapacity = _arrayFromPool.Length * 2;
            T[] newArray = ArrayPool<T>.Shared.Rent(newCapacity);

            Array.Copy(_arrayFromPool, newArray, _count);
            ArrayPool<T>.Shared.Return(_arrayFromPool);
            _arrayFromPool = newArray;
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
    }
}
