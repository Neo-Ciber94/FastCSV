using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FastCSV.Internal;

namespace FastCSV
{
    /// <summary>
    /// Represents a contiguous range of columns of a CSV record.
    /// </summary>
    public readonly ref struct CsvRow
    {
        /// <summary>
        /// An empty <see cref="CsvRow"/>.
        /// </summary>
        public static CsvRow Empty => default;

        private readonly ReadOnlySpan<string> _values;
        private readonly ReadOnlySpan<string> _header;
        private readonly CsvFormat? _format;

        public CsvRow(CsvRecord record) : this(record, 0, record.Length) { }

        public CsvRow(CsvRecord record, int startIndex) : this(record, startIndex, record.Length - startIndex) { }

        public CsvRow(CsvRecord record, int startIndex, int count)
        {
            _values = record._values.AsSpan(startIndex, count);
            _header = record.Header == null ? Array.Empty<string>() : record.Header._values.AsSpan(startIndex, count);
            _format = record.Format;
        }

        public CsvRow(CsvHeader header) : this(header, 0, header.Length) { }

        public CsvRow(CsvHeader header, int startIndex) : this(header, startIndex, header.Length - startIndex) { }

        public CsvRow(CsvHeader header, int startIndex, int count)
        {
            _values = header._values.AsSpan(startIndex, count);
            _header = Array.Empty<string>();
            _format = header.Format;
        }

        internal CsvRow(ReadOnlySpan<string> values, ReadOnlySpan<string> header, CsvFormat format)
        {
            Requires.True(values.Length == header.Length);

            _values = values;
            _header = header;
            _format = format;
        }

        /// <summary>
        /// The number of columns in this row.
        /// </summary>
        public int Length => _values.Length;

        /// <summary>
        /// Check if this row is empty.
        /// </summary>
        public bool IsEmpty => _values.IsEmpty;

        /// <summary>
        /// The csv format of this row.
        /// </summary>
        public CsvFormat Format => _format ?? CsvFormat.Default;

        /// <summary>
        /// The header column values.
        /// </summary>
        public ReadOnlySpan<string> Header => _header;

        /// <summary>
        /// The row column values.
        /// </summary>
        public ReadOnlySpan<string> Values => _values;

        /// <summary>
        /// Get the value of the specified column index.
        /// </summary>
        public string this[int index] => _values[index];

        /// <summary>
        /// Get the value of the specified column index.
        /// </summary>
        public string this[Index index] => _values[index];

        /// <summary>
        /// Gets the <see cref="CsvRow"/> from the given range.
        /// </summary>
        /// <returns></returns>
        public CsvRow this[Range range]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Slice(range);
        }

        /// <summary>
        /// Gets the value with the given key, or null if not found.
        /// </summary>
        /// <value></value>
        public string? this[string key]
        {
            get
            {
                if (Header.IsEmpty)
                {
                    return null;
                }

                var index = _header.IndexOf(key);

                if (index == -1)
                {
                    return null;
                }

                return _values[index];
            }
        }

        /// <summary>
        /// Gets a new <see cref="CsvRow"/> from the given range.
        /// </summary>
        /// <param name="startIndex">The start column.</param>
        /// <returns></returns>
        public CsvRow Slice(int startIndex)
        {
            var values = _values[startIndex..];
            var header = _header.IsEmpty ? _header : _header[startIndex..];
            return new CsvRow(values, header, _format!);
        }

        /// <summary>
        /// Gets a new <see cref="CsvRow"/> from the given range.
        /// </summary>
        /// <param name="startIndex">The start column.</param>
        /// <param name="count">The number of columns</param>
        /// <returns></returns>
        public CsvRow Slice(int startIndex, int count)
        {
            var values = _values.Slice(startIndex, count);
            var header = _header.IsEmpty ? _header : _header.Slice(startIndex, count);
            return new CsvRow(values, header, _format!);
        }

        /// <summary>
        /// Gets a new <see cref="CsvRow"/> from the given range.
        /// </summary>
        /// <param name="index">The start column.</param>
        /// <returns></returns>
        public CsvRow Slice(Index index)
        {
            int actualIndex = index.GetOffset(_values.Length);
            return this[actualIndex..];
        }

        /// <summary>
        /// Gets a new <see cref="CsvRow"/> from the given range.
        /// </summary>
        /// <param name="range">The range of columns</param>
        /// <returns></returns>
        public CsvRow Slice(Range range)
        {
            var values = _values[range];
            var header = _header.IsEmpty ? _header : _header[range];
            return new CsvRow(values, header, _format!);
        }

        /// <summary>
        /// Gets a copy of this <see cref="CsvRow"/> with the given format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public CsvRow WithFormat(CsvFormat format)
        {
            return new CsvRow(_values, _header, format);
        }

        /// <summary>
        /// Gets an enumerator over the values of this row.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator() => new(_values);

        /// <summary>
        /// Gets a string representation of this row.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(_format!);
        }

        /// <summary>
        /// Gets a string representation of this row.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns></returns>
        public string ToString(CsvFormat format)
        {
            return CsvUtility.ToCsvString(_values, format);
        }

        public static implicit operator CsvRow(CsvRecord record)
        {
            return new CsvRow(record);
        }

        public static implicit operator CsvRow(CsvHeader header)
        {
            return new CsvRow(header);
        }

        public static bool operator ==(CsvRow left, CsvRow right)
        {
            return left._values.SequenceEqual(right._values);
        }

        public static bool operator !=(CsvRow left, CsvRow right)
        {
            return !(left == right);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj)
        {
            throw new NotSupportedException($"{typeof(CsvRow)} does not support {nameof(Equals)}, use equality operator instead.");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            throw new NotSupportedException($"{typeof(CsvRow)} does not support {nameof(GetHashCode)}");
        }

        public ref struct Enumerator
        {
            private readonly ReadOnlySpan<string> _values;
            private int _index;

            public Enumerator(ReadOnlySpan<string> values)
            {
                _values = values;
                _index = -1;
            }

            public bool MoveNext()
            {
                if (_index < _values.Length - 1)
                {
                    _index++;
                    return true;
                }

                return false;
            }

            public ref readonly string Current => ref _values[_index];

            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
