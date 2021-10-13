using FastCSV.Utils;
using System;

namespace FastCSV.Collections
{
    public struct MemoryIterator<T> where T: notnull
    {
        private readonly ReadOnlyMemory<T> _memory;
        private int _pos;

        public MemoryIterator(ReadOnlyMemory<T> memory)
        {
            _memory = memory;
            _pos = -1;
        }

        public T Current => _memory.Span[_pos];

        public Optional<T> Peek
        {
            get
            {
                int length = _memory.Length;
                int next = _pos + 1;

                if (next < length)
                {
                    return _memory.Span[next];
                }

                return default;
            }
        }

        public bool MoveNext()
        {
            int next = _pos + 1;

            if (next < _memory.Length)
            {
                _pos = next;
                return true;
            }

            return false;
        }

        public bool HasNext()
        {
            int next = _pos + 1;
            return next < _memory.Length;
        }
        
        public MemoryIterator<T> GetEnumerator() => this;
    }
}
