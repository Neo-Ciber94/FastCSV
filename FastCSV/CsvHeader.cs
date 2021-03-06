﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Represents the header of a csv document.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerable{System.String}" />
    /// <seealso cref="FastCSV.ICloneable{FastCSV.CsvHeader}" />
    /// <seealso cref="System.IEquatable{FastCSV.CsvHeader}" />
    [Serializable]
    public class CsvHeader : IEnumerable<string>, ICloneable<CsvHeader>, IEquatable<CsvHeader?>
    {
        private readonly string[] _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvHeader"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public CsvHeader(IEnumerable<string> values) : this(values, CsvFormat.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvHeader"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="format">The format.</param>
        public CsvHeader(IEnumerable<string> values, CsvFormat format)
        {
            string[] array = values.ToArray();

            if(array.Length == 0)
            {
                throw new ArgumentException("Header need at least 1 value");
            }

            _values = array;
            Format = format;
        }

        /// <summary>
        /// Creates a csv header from the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>A csv header with the specified values</returns>
        public static CsvHeader FromValues(params string[] values)
        {
            return new CsvHeader(values);
        }

        /// <summary>
        /// Creates a csv header from the specified values and format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns>A csv header with the specified values</returns>
        public static CsvHeader FromValues(CsvFormat format, params string[] values)
        {
            return new CsvHeader(values, format);
        }

        /// <summary>
        /// Creates a csv header for the specified type.
        /// </summary>
        /// <typeparam name="T">Type used to create the header</typeparam>
        /// <returns>The header using the names of the public fields and properties of the specified type.</returns>
        public static CsvHeader FromType<T>()
        {
            return FromType<T>(CsvFormat.Default);
        }

        /// <summary>
        /// Creates a csv header for the specified type.
        /// </summary>
        /// <typeparam name="T">Type used to create the header</typeparam>
        /// <param name="format">The format.</param>
        /// <returns>The header using the names of the public fields and properties of the specified type.</returns>
        public static CsvHeader FromType<T>(CsvFormat format)
        {
            var values = CsvUtility.GetHeader<T>();
            return new CsvHeader(values, format);
        }

        /// <summary>
        /// Gets the number of fields in the header.
        /// </summary>
        /// <value>
        /// The number of fields in the header.
        /// </value>
        public int Length => _values.Length;

        /// <summary>
        /// Gets the format of the header.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public CsvFormat Format { get; }

        /// <summary>
        /// Gets field at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The field at the specified index.</returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
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
        /// Gets the fields at the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>The values at the specified range.</returns>
        public Span<string> this[Range range] => _values.AsSpan(range);

        /// <summary>
        /// Gets the index of the specified value in this header.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The index of the value or -1 if not found.</returns>
        public int IndexOf(string value)
        {
            return Array.IndexOf(_values, value);
        }

        /// <summary>
        /// Gets a copy of this header using the specified delimiter.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>A copy of this header with the specified delimiter.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvHeader WithDelimiter(char delimiter)
        {
            return WithFormat(Format.WithDelimiter(delimiter));
        }

        /// <summary>
        /// Gets a copy of this header using the specified quote.
        /// </summary>
        /// <param name="quote">The quote.</param>
        /// <returns>A copy of this header with the specified quote.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvHeader WithQuote(char quote)
        {
            return WithFormat(Format.WithQuote(quote));
        }

        /// <summary>
        /// Gets a copy of this header using the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>A copy of this header with the specified style.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvHeader WithStyle(QuoteStyle style)
        {
            return WithFormat(Format.WithStyle(style));
        }

        /// <summary>
        /// Gets a copy of this header using the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A copy of this header with the specified format.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvHeader WithFormat(CsvFormat format)
        {
            return new CsvHeader(_values, format);
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

        public CsvHeader Clone()
        {
            string[] values = (string[])_values.Clone();
            return new CsvHeader(values, Format);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CsvHeader);
        }

        public bool Equals(CsvHeader? other)
        {
            return other != null &&
                   _values.SequenceEqual(other._values) &&
                   EqualityComparer<CsvFormat>.Default.Equals(Format, other.Format);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_values, Format);
        }

        public static bool operator ==(CsvHeader? left, CsvHeader? right)
        {
            return EqualityComparer<CsvHeader?>.Default.Equals(left, right);
        }

        public static bool operator !=(CsvHeader? left, CsvHeader? right)
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
                if (i < _values.Length)
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
