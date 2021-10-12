using System;

namespace FastCSV.Utils
{
    public ref struct SpanIterator<T> where T: notnull
    {
        private readonly ReadOnlySpan<T> _span;
        private int _pos;

        public SpanIterator(ReadOnlySpan<T> span)
        {
            _span = span;
            _pos = -1;
        }

        public T Current => _span[_pos];

        public Optional<T> Peek
        {
            get
            {
                int length = _span.Length;

                if (_pos >= 0 && _pos < length)
                {
                    int next = _pos + 1;
                    if (next < length)
                    {
                        return _span[next];
                    }
                }

                return default;
            }
        }

        public bool MoveNext()
        {
            int next = _pos + 1;

            if (next < _span.Length)
            {
                _pos = next;
                return true;
            }

            return false;
        }

        public bool HasNext()
        {
            int next = _pos + 1;
            return next < _span.Length;
        }
        
        public SpanIterator<T> GetEnumerator() => this;
    }
}
