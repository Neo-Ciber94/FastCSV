using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using FastCSV.Utils;

namespace FastCSV
{
    public class CsvDocument<T> : ICsvDocument
    {
        private readonly List<TypedCsvRecord<T>> _records = new List<TypedCsvRecord<T>>();

        public CsvDocument() : this(CsvFormat.Default) { }

        public CsvDocument(CsvFormat format)
        {
            Header = new CsvHeader(CsvUtility.GetHeader<T>(), format);
            Format = format;
        }

        public CsvDocument(IEnumerable<T> elements) : this(elements, CsvFormat.Default) { }

        public CsvDocument(IEnumerable<T> elements, CsvFormat format)
        {
            Header = new CsvHeader(CsvUtility.GetHeader<T>(), format);
            Format = format;

            foreach(var e in elements)
            {
                Write(e);
            }
        }

        public CsvRecord this[int index] => _records[index].Record;

        public CsvHeader Header { get; }

        public CsvFormat Format { get; }

        public int Count => _records.Count;

        public bool IsEmpty => Count == 0;

        public ValueCollection Values => new ValueCollection(_records);

        public void Write(T value)
        {
            _records.Add(new TypedCsvRecord<T>(value, Header, Format));
        }

        public void WriteAt(int index, T value)
        {
            _records.Insert(index, new TypedCsvRecord<T>(value, Header, Format));
        }

        public void Update(int index, T value)
        {
            _records[index] = new TypedCsvRecord<T>(value, Header, Format);
        }

        public CsvRecord RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(index.ToString());
            }

            TypedCsvRecord<T> typedRecord = _records[index];
            _records.RemoveAt(index);
            return typedRecord.Record;
        }

        public int RemoveAll(Predicate<T> match)
        {
            return _records.RemoveAll((n) => match(n.Value));
        }

        public void Clear()
        {
            _records.Clear();
        }

        public bool Contains(T value)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            foreach (TypedCsvRecord<T> e in _records)
            {
                if (comparer.Equals(e.Value, value))
                {
                    return true;
                }
            }

            return false;
        }

        public int IndexOf(T value)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            int length = _records.Count;

            for(int i = 0; i < length; i++)
            {
                T current = _records[i].Value;

                if(comparer.Equals(current, value))
                {
                    return i;
                }
            }

            return -1;
        }

        public int LastIndexOf(T value)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            int length = _records.Count;

            for (int i = length - 1; i >= 0; i--)
            {
                T current = _records[i].Value;

                if (comparer.Equals(current, value))
                {
                    return i;
                }
            }

            return -1;
        }

        public T GetValue(int index)
        {
            if(index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException(index.ToString());
            }

            return _records[index].Value;
        }

        public string ToString(CsvFormat format)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Header.ToString(format));

            foreach (TypedCsvRecord<T> typedRecord in _records)
            {
                sb.AppendLine(typedRecord.Record.ToString(format));
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString(Format);
        }

        public Enumerator GetEnumerator() => new Enumerator(_records);

        IEnumerator<CsvRecord> IEnumerable<CsvRecord>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<CsvRecord>
        {
            private readonly List<TypedCsvRecord<T>> _records;
            private int _index;

            internal Enumerator(List<TypedCsvRecord<T>> records)
            {
                _records = records;
                _index = -1;
            }

            public CsvRecord Current => _records[_index].Record;

            object? IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                int next = _index + 1;

                if(next < _records.Count)
                {
                    _index = next;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        public struct ValueCollection : IReadOnlyList<T>
        {
            private readonly List<TypedCsvRecord<T>> _records;

            internal ValueCollection(List<TypedCsvRecord<T>> records)
            {
                _records = records;
            }

            public int Count => _records.Count;

            public T this[int index] => _records[index].Value;

            public IEnumerator<T> GetEnumerator() => _records.Select(e => e.Value).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
