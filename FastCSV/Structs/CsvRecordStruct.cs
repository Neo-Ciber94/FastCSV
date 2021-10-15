using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FastCSV.Collections;
using FastCSV.Utils;
using FastCSV.Extensions;

namespace FastCSV.Struct
{
    /// <summary>
    /// Represents a record in a csv document.
    /// </summary>
    /// <seealso cref="FastCSV.ICsvRecord" />
    /// <seealso cref="ICloneable{CsvRecordStruct}" />
    /// <seealso cref="IEquatable{CsvRecordStruct}" />
    [Serializable]
    public readonly struct CsvRecordStruct : IEnumerable<string>, ICloneable<CsvRecordStruct>, IEquatable<CsvRecordStruct>
    {
        private readonly ReadOnlyArray<string> _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRecordStruct"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="values">The values.</param>
        public CsvRecordStruct(CsvHeaderStruct? header, IEnumerable<string> values) : this(header, values, header?.Format ?? CsvFormat.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRecordStruct"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="values">The values.</param>
        /// <param name="format">The format.</param>
        /// <exception cref="ArgumentException">If the header csv format is different than the provided format</exception>
        public CsvRecordStruct(CsvHeaderStruct? header, IEnumerable<string> values, CsvFormat format)
        {
            if (header != null && header.Value.Format != format)
            {
                throw new ArgumentException("Header csv format is different than the provided format");
            }

            _values = values.ToArray();
            Header = header;
            Format = format;
        }

        internal CsvRecordStruct(CsvHeaderStruct? header, string[] values, CsvFormat format)
        {
            if (header != null && header.Value.Format != format)
            {
                throw new ArgumentException("Header csv format is different than the provided format");
            }

            _values = values;
            Header = header;
            Format = format;
        }

        internal CsvRecordStruct(CsvHeaderStruct? header, ReadOnlyArray<string> values, CsvFormat format)
        {
            if (header != null && header.Value.Format != format)
            {
                throw new ArgumentException("Header csv format is different than the provided format");
            }

            _values = values;
            Header = header;
            Format = format;
        }


        /// <summary>
        /// Creates a record from the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static CsvRecordStruct From<T>(T value)
        {
            return From(value, CsvFormat.Default);
        }

        /// <summary>
        /// Creates a record from the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static CsvRecordStruct From<T>(T value, CsvFormat format)
        {
            var headerValues = CsvConverter.GetHeader<T>();
            var values = CsvConverter.GetValues(value);

            var header = new CsvHeaderStruct(headerValues, format);
            return new CsvRecordStruct(header, values, format);
        }

        /// <summary>
        /// Gets the number of fields in this record.
        /// </summary>
        /// <value>
        /// The number of fields in this csv record.
        /// </value>
        public int Length => _values.Count;

        /// <summary>
        /// Gets the csv format of this record.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public CsvFormat Format { get; }

        /// <summary>
        /// Gets the header (if any) of the csv this record belongs.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public CsvHeaderStruct? Header { get; }

        /// <summary>
        /// Gets the field at the specified index.
        /// </summary>
        /// <value>
        /// The field in the specified index.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= _values.Count)
                {
                    throw new ArgumentOutOfRangeException($"{index} > {Length}");
                }

                return _values[index];
            }
        }

        /// <summary>
        /// Gets the field with the specified key.
        /// </summary>
        /// <value>
        /// The value corresponding to the specified key.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If the header is not set</exception>
        /// <exception cref="KeyNotFoundException">If cannot find a value for the corresponding key: {key}</exception>
        public string this[string key]
        {
            get
            {
                if (Header is null)
                {
                    throw new InvalidOperationException("Header is not set");
                }

                int index = Header.Value.IndexOf(key);

                return index >= 0 ? this[index] : throw new KeyNotFoundException($"Cannot find a value for the corresponding key: {key}");
            }
        }

        /// <summary>
        /// Gets the values at the specified range.
        /// </summary>
        /// <value>
        /// The values at the specified range.
        /// </value>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public ReadOnlyMemory<string> this[Range range] => _values.AsMemory(range);

        /// <summary>
        /// Gets a <see cref="ReadOnlyMemory{T}"/> view to the elements of this record.
        /// </summary>
        public ReadOnlyMemory<string> AsMemory()
        {
            return _values.AsMemory();
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlySpan{T}"/> view to the elements of this record.
        /// </summary>
        public ReadOnlySpan<string> AsSpan()
        {
            return _values.AsSpan();
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyDictionary{TKey, TValue}"/> with the values of the specified column names.
        /// </summary>
        /// <param name="columnNames">The column names, this value can be implicit converted from <see cref="string"/>.</param>
        /// <returns>Gets the values of the specified column names.</returns>
        public IReadOnlyDictionary<string, string> GetValues(params ColumnName[] columnNames)
        {
            if (columnNames.Length == 0)
            {
                return EmptyDictionary<string, string>.Value;
            }

            var result = new Dictionary<string, string>(columnNames.Length);

            foreach (ColumnName columnName in columnNames)
            {
                result.Add(columnName.GetAliasOrName(), this[columnName.Name]);
            }

            return result;
        }

        /// <summary>
        /// Gets a copy of this record using the specified delimiter.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>A copy of this record with the specified delimiter.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecordStruct WithDelimiter(char delimiter)
        {
            return WithFormat(Format.WithDelimiter(delimiter));
        }

        /// <summary>
        /// Gets a copy of this record using the specified quote.
        /// </summary>
        /// <param name="quote">The quote.</param>
        /// <returns>A copy of this record with the specified quote.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecordStruct WithQuote(char quote)
        {
            return WithFormat(Format.WithQuote(quote));
        }

        /// <summary>
        /// Gets a copy of this record using the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>A copy of this record with the specified style.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecordStruct WithStyle(QuoteStyle style)
        {
            return WithFormat(Format.WithStyle(style));
        }

        /// <summary>
        /// Gets a copy of this record using the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A copy of this record with the specified format.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecordStruct WithFormat(CsvFormat format)
        {
            return new CsvRecordStruct(Header?.WithFormat(format), _values, format);
        }

        /// <summary>
        /// Converts this record to a dictionary if has a header.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <returns>A dictionary containing the key and values of this record, or null if this record don't have a header</returns>
        public Dictionary<string, SingleOrList<string>>? ToDictionary(IEqualityComparer<string>? comparer = null)
        {
            if (Header == null)
            {
                return null;
            }

            Dictionary<string, SingleOrList<string>> result = new(Length, comparer);

            foreach (string key in Header)
            {
                result.Add(key, this[key]);
            }

            return result;
        }

        /// <summary>
        /// Mutates the contents of this instance and get the mutated instance.
        /// </summary>
        /// <param name="action">The action to mutate the record.</param>
        /// <returns>The mutated instance.</returns>
        public CsvRecordStruct Mutate(Action<Mutable> action)
        {
            var mutable = new Mutable(this);
            action(mutable);
            return mutable.ToRecord();
        }

        public string ToString(CsvFormat format)
        {
            return CsvUtility.ToCsvString(_values, format);
        }

        public override string ToString()
        {
            return ToString(Format);
        }

        public Enumerator GetEnumerator() => new Enumerator(_values);

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public CsvRecordStruct Clone()
        {
            CsvHeaderStruct? header = Header?.Clone();
            return new CsvRecordStruct(header, _values, Format);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_values, Length, Format, Header);
        }

        public override bool Equals(object? obj)
        {
            return obj is CsvRecordStruct @struct && Equals(@struct);
        }

        public bool Equals(CsvRecordStruct other)
        {
            return _values == other._values &&
                   EqualityComparer<CsvFormat>.Default.Equals(Format, other.Format) &&
                   EqualityComparer<CsvHeaderStruct?>.Default.Equals(Header, other.Header);
        }

        public static bool operator ==(CsvRecordStruct? left, CsvRecordStruct? right)
        {
            return EqualityComparer<CsvRecordStruct?>.Default.Equals(left, right);
        }

        public static bool operator !=(CsvRecordStruct? left, CsvRecordStruct? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Provides a way to mutate a <see cref="CsvRecordStruct"/>.
        /// </summary>
        public readonly struct Mutable
        {
            private readonly string[] _values;
            private readonly CsvHeaderStruct? _header;
            private readonly CsvFormat _format;

            internal Mutable(CsvRecordStruct record)
            {
                _values = new string[record.Length];
                _header = record.Header;
                _format = record.Format;

                record._values.AsSpan().CopyTo(_values);
            }

            public int Length => _values.Length;

            public string this[int index]
            {
                get
                {
                    if (index < 0 || index >= _values.Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }

                    return _values[index];
                }

                set
                {
                    if (index < 0 || index >= _values.Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }

                    _values[index] = value;
                }
            }

            public string this[string key]
            {
                get
                {
                    if (_header == null)
                    {
                        throw new InvalidOperationException("Record don't have a header");
                    }

                    int index = _header.Value.IndexOf(key);
                    if (index >= 0)
                    {
                        return _values[index];
                    }
                    else
                    {
                        throw new ArgumentException("Key not found: " + key);
                    }
                }

                set
                {
                    if (_header == null)
                    {
                        throw new InvalidOperationException("Record don't have a header");
                    }

                    int index = _header.Value.IndexOf(key);
                    if (index >= 0)
                    {
                        _values[index] = value;
                    }
                    else
                    {
                        throw new KeyNotFoundException(key);
                    }
                }
            }

            public void Update(int index, object? value)
            {
                this[index] = value.ToStringOrEmpty();
            }

            public void Update(string key, object? value)
            {
                this[key] = value.ToStringOrEmpty();
            }

            internal CsvRecordStruct ToRecord()
            {
                return new CsvRecordStruct(_header, _values, _format);
            }
        }

        /// <summary>
        /// An enumerator over the fields of this record.
        /// </summary>
        /// <seealso cref="IEnumerator{string}" />
        public struct Enumerator : IEnumerator<string>
        {
            private readonly ReadOnlyArray<string> _values;
            private int _index;

            internal Enumerator(ReadOnlyArray<string> values)
            {
                _values = values;
                _index = -1;
            }

            public string Current => _values[_index];

            object? IEnumerator.Current => Current;

            public bool MoveNext()
            {
                int i = _index + 1;
                if (i < _values.Count)
                {
                    _index = i;
                    return true;
                }

                return false;
            }

            public void Dispose() { }

            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
