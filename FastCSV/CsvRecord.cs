﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class CsvRecord : ICsvRecord, ICloneable<CsvRecord>, IEquatable<CsvRecord?>
    {
        private readonly string[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRecord"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="values">The values.</param>
        public CsvRecord(CsvHeader? header, IEnumerable<string> values) : this(header, values, CsvFormat.Default) { }

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
                if (index < 0 || index > _values.Length)
                {
                    throw new IndexOutOfRangeException($"{index} > {Length}");
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
        public CsvRecord WithDelimiter(char delimiter)
        {
            return new CsvRecord(Header, _values, Format.WithDelimiter(delimiter));
        }

        /// <summary>
        /// Gets a copy of this record using the specified quote.
        /// </summary>
        /// <param name="quote">The quote.</param>
        /// <returns>A copy of this record with the specified quote.</returns>
        public CsvRecord WithQuote(char quote)
        {
            return new CsvRecord(Header, _values, Format.WithQuote(quote));
        }

        /// <summary>
        /// Gets a copy of this record using the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>A copy of this record with the specified style.</returns>
        public CsvRecord WithStyle(QuoteStyle style)
        {
            return new CsvRecord(Header, _values, Format.WithStyle(style));
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
