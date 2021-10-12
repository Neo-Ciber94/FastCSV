using System;
using System.Buffers;

namespace FastCSV.Collections
{
    internal struct ArrayBuilder<T> : IDisposable
    {
        private T[]? _array;
        private int _count;

        public ArrayBuilder(int initialCapacity)
        {
            _array = ArrayPool<T>.Shared.Rent(initialCapacity);
            _count = 0;
        }

        public int Count => _count;

        public int Capacity => _array?.Length ?? 0;

        public void Add(T value)
        {
            if (_array == null)
            {
                return;
            }

            if (_count == _array.Length)
            {
                Resize();
            }

            _array[_count++] = value;
        }

        public T[] Build()
        {
            if (_array == null)
            {
                return Array.Empty<T>();
            }

            return _array.AsSpan(0, _count).ToArray();
        }

        private void Resize()
        {
            if (_array == null)
            {
                return;
            }

            // Returns the array
            ArrayPool<T>.Shared.Return(_array);

            int newCapacity = _array.Length * 2;
            _array = ArrayPool<T>.Shared.Rent(newCapacity);
        }

        public void Dispose()
        {
            if (_array != null)
            {
                ArrayPool<T>.Shared.Return(_array);
                _array = null;
                _count = 0;
            }
        }
    }
}
