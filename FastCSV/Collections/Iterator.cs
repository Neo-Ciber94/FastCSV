using System.Collections;
using System.Collections.Generic;
using FastCSV.Utils;

namespace FastCSV.Collections
{
    public struct Iterator<T> : IIterator<T> where T : notnull
    {
        private readonly IEnumerator<T> _enumerator;
        private Optional<T> _next;
        private T _current;

        public Iterator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
            _current = default!;
            _next = default;
        }

        public T Current => _current;

        object IEnumerator.Current => Current!;

        public Optional<T> Peek
        {
            get
            {
                HasNext();
                return _next;
            }
        }

        public bool HasNext()
        {
            return MoveNext(moving: false);
        }

        public bool MoveNext()
        {
            return MoveNext(moving: true);
        }

        private bool MoveNext(bool moving)
        {
            if (moving)
            {
                if (_next.HasValue)
                {
                    _current = _next.Value;
                    _next = default;
                    return true;
                }
                else
                {
                    if (_enumerator.MoveNext())
                    {
                        _current = _enumerator.Current;
                        return true;
                    }

                    return false;
                }
            }
            else
            {
                if (_next.HasValue)
                {
                    return true;
                }
                else
                {
                    if (_enumerator.MoveNext())
                    {
                        var temp = _enumerator.Current;
                        _next = new Optional<T>(temp);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public void Reset()
        {
            _current = default!;
            _next = default;
            _enumerator.Reset();
        }
    }
}
