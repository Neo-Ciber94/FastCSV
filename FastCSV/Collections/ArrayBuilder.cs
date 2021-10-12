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

        public T[] Build()
        {
            if (_arrayFromPool == null)
            {
                return Array.Empty<T>();
            }

            return _arrayFromPool.AsSpan(0, _count).ToArray();
        }

        private void Resize()
        {
            if (_arrayFromPool == null)
            {
                return;
            }

            // Returns the array
            ArrayPool<T>.Shared.Return(_arrayFromPool);

            int newCapacity = _arrayFromPool.Length * 2;
            _arrayFromPool = ArrayPool<T>.Shared.Rent(newCapacity);
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
