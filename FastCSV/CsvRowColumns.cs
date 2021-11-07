using System;
using System.Collections;
using System.Collections.Generic;
using FastCSV.Collections;

namespace FastCSV
{
    public readonly struct CsvRowColumns : IEquatable<CsvRowColumns>, IEnumerable<string>
    {
        public static CsvRowColumns Empty { get; } = default;

        private readonly CsvRecord? _record;
        private readonly ReadOnlyArray<string> _columnNames;

        public CsvRowColumns(CsvRecord record, ReadOnlyArray<string> columnNames)
        {
            if (columnNames.Count == 0)
            {
                throw new ArgumentException("Column names cannot be empty");
            }

            if (record.Header == null)
            {
                throw new ArgumentException("Record does not have a header");
            }

            _record = record; ;
            _columnNames = columnNames;
        }

        public int Count => _columnNames.Count;

        public bool IsEmpty => _columnNames.Count == 0;

        public ReadOnlySpan<string> ColumnNames => _columnNames.AsSpan();

        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= _columnNames.Count || _record == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                int actualIndex = _record.Header!.IndexOf(_columnNames[index]);
                return _record[actualIndex];
            }
        }

        public string? this[string columnName]
        {
            get
            {
                int index = _columnNames.IndexOf(columnName);
                return index == -1 ? null : this[index];
            }
        }

        public bool Equals(CsvRowColumns other)
        {
            if (Count != other.Count)
            {
                return false;
            }

            var comparer = EqualityComparer<string>.Default;

            for (int i = 0; i < Count; i++)
            {
                if (!comparer.Equals(this[i], other[i]) || !comparer.Equals(_columnNames[i], other._columnNames[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is CsvRowColumns other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            for (int i = 0; i < Count; i++)
            {
                hashCode.Add(_columnNames[i]);
                hashCode.Add(this[i]);
            }

            return hashCode.ToHashCode();
        }

        public string ToString(CsvFormat format)
        {
            string[] values = new string[_columnNames.Count];
            for (int i = 0; i < _columnNames.Count; i++)
            {
                values[i] = this[i]!;
            }

            return CsvUtility.ToCsvString(values, format);
        }

        public override string ToString()
        {
            return ToString(CsvFormat.Default);
        }

        public static bool operator ==(CsvRowColumns left, CsvRowColumns right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CsvRowColumns left, CsvRowColumns right)
        {
            return !left.Equals(right);
        }

        public Enumerator GetEnumerator() => new(this);

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<string>
        {
            private readonly CsvRowColumns _columns;
            private int _index;

            public Enumerator(CsvRowColumns columns)
            {
                _columns = columns;
                _index = -1;
            }

            public string Current => _columns[_index];

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_index < _columns.Count - 1)
                {
                    _index++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose()
            {
            }
        }
    }
}