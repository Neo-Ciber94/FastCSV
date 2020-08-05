using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Represents a record in a csv document.
    /// </summary>
    /// <seealso cref="FastCSV.ICsvRecord" />
    /// <seealso cref="FastCSV.ICloneable{FastCSV.CsvRecord}" />
    /// <seealso cref="System.IEquatable{FastCSV.CsvRecord}" />
    [Serializable]
    public class CsvRecord : IEnumerable<string>, ICloneable<CsvRecord>, IEquatable<CsvRecord?>
    {
        private readonly string[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRecord"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="values">The values.</param>
        public CsvRecord(CsvHeader? header, IEnumerable<string> values) : this(header, values, header?.Format?? CsvFormat.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRecord"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="values">The values.</param>
        /// <param name="format">The format.</param>
        /// <exception cref="ArgumentException">If the header csv format is different than the provided format</exception>
        public CsvRecord(CsvHeader? header, IEnumerable<string> values, CsvFormat format)
        {
            if(header != null && header.Format != format)
            {
                throw new ArgumentException("Header csv format is different than the provided format");
            }

            _values = values.ToArray();
            Header = header;
            Format = format;
        }

        internal CsvRecord(CsvHeader? header, string[] values, CsvFormat format)
        {
            if (header != null && header.Format != format)
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
        public static CsvRecord From<T>(T value)
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
        public static CsvRecord From<T>(T value, CsvFormat format)
        {
            var headerValues = CsvUtility.GetHeader<T>();
            var values = CsvUtility.GetValues(value);

            var header = new CsvHeader(headerValues, format);
            return new CsvRecord(header, values, format);
        }

        /// <summary>
        /// Gets the number of fields in this record.
        /// </summary>
        /// <value>
        /// The number of fields in this csv record.
        /// </value>
        public int Length => _values.Length;

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
        public CsvHeader? Header { get; }

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
                if (index < 0 || index >= _values.Length)
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

                int index = Header.IndexOf(key);              

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
        public Span<string> this[Range range] => _values.AsSpan(range);

        /// <summary>
        /// Gets a copy of this record using the specified delimiter.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>A copy of this record with the specified delimiter.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecord WithDelimiter(char delimiter)
        {
            return WithFormat(Format.WithDelimiter(delimiter));
        }

        /// <summary>
        /// Gets a copy of this record using the specified quote.
        /// </summary>
        /// <param name="quote">The quote.</param>
        /// <returns>A copy of this record with the specified quote.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecord WithQuote(char quote)
        {
            return WithFormat(Format.WithQuote(quote));
        }

        /// <summary>
        /// Gets a copy of this record using the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>A copy of this record with the specified style.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecord WithStyle(QuoteStyle style)
        {
            return WithFormat(Format.WithStyle(style));
        }

        /// <summary>
        /// Gets a copy of this record using the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A copy of this record with the specified format.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecord WithFormat(CsvFormat format)
        {
            return new CsvRecord(Header?.WithFormat(format), _values, format);
        }

        /// <summary>
        /// Converts this record to a dictionary if has a header.
        /// </summary>
        /// <returns>A dictionary containing the key and values of this record, or null if this record don't have a header</returns>
        public Dictionary<string, string>? ToDictionary()
        {
            return ToDictionary(null);
        }

        /// <summary>
        /// Converts this record to a dictionary if has a header.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <returns>A dictionary containing the key and values of this record, or null if this record don't have a header</returns>
        public Dictionary<string, string>? ToDictionary(IEqualityComparer<string>? comparer)
        {
            if(Header == null)
            {
                return null;
            }

            Dictionary<string, string> result = new Dictionary<string, string>(Length, comparer);

            foreach(string key in Header)
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
        public CsvRecord Mutate(Action<Mutable> action)
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

        public CsvRecord Clone()
        {
            CsvHeader? header = Header?.Clone();
            string[]? values = _values.ToArray();
            return new CsvRecord(header, values, Format);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CsvRecord);
        }

        public bool Equals(CsvRecord? other)
        {
            return other != null &&
                   _values.SequenceEqual(other._values) &&
                   Length == other.Length &&
                   EqualityComparer<CsvFormat>.Default.Equals(Format, other.Format) &&
                   EqualityComparer<CsvHeader?>.Default.Equals(Header, other.Header);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_values, Length, Format, Header);
        }

        public static bool operator ==(CsvRecord? left, CsvRecord? right)
        {
            return EqualityComparer<CsvRecord?>.Default.Equals(left, right);
        }

        public static bool operator !=(CsvRecord? left, CsvRecord? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Provides a way to mutate a <see cref="CsvRecord"/>.
        /// </summary>
        public readonly struct Mutable
        {
            private readonly string[] _values;
            private readonly CsvHeader? _header;
            private readonly CsvFormat _format;

            internal Mutable(CsvRecord record)
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
                    if(index < 0 || index >= _values.Length)
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
                    if(_header == null)
                    {
                        throw new InvalidOperationException("Record don't have a header");
                    }

                    int index = _header.IndexOf(key);
                    if(index >= 0)
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

                    int index = _header.IndexOf(key);
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

            internal CsvRecord ToRecord()
            {
                return new CsvRecord(_header, _values, _format);
            }
        }

        /// <summary>
        /// An enumerator over the fields of this record.
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEnumerator{System.String}" />
        public struct Enumerator : IEnumerator<string>
        {
            private readonly string[] _values;
            private int _index;

            internal Enumerator(string[] values)
            {
                _values = values;
                _index = -1;
            }

            public string Current => _values[_index];

            object? IEnumerator.Current => Current;

            public bool MoveNext()
            {
                int i = _index + 1;
                if(i < _values.Length)
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
