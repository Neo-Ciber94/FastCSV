using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV
{
    public partial class CsvDocument<T>
    {
        public readonly struct ValueCollection : IReadOnlyList<T>
        {
            private readonly CsvDocument<T> _document;

            internal ValueCollection(CsvDocument<T> document)
            {
                _document = document;
            }

            public readonly int Count => _document._count;

            public readonly T this[int index] => _document.GetValue(index);

            public ValueCollectionEnumerator GetEnumerator()
            {
                return new ValueCollectionEnumerator(_document);
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct ValueCollectionEnumerator : IEnumerator<T>
        {
            private readonly TypedRecord[] _values;
            private readonly int _count;
            private int _index;

            public ValueCollectionEnumerator(CsvDocument<T> document)
            {
                _values = document._records;
                _count = document._count;
                _index = -1;
            }

            public T Current
            {
                get
                {
                    if (_index == -1)
                    {
                        throw new InvalidOperationException("enumerator is no initialized");
                    }

                    return _values[_index].Value;
                }
            }

            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                int next = _index + 1;
                bool hasNext = next < _count;

                if (hasNext)
                {
                    _index = next;
                }

                return hasNext;
            }

            public void Reset()
            {
                _index = -1;
            }

            void IDisposable.Dispose() { }
        }
    }
}
