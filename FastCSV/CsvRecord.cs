using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FastCSV.Collections;
using FastCSV.Utils;
using FastCSV.Extensions;

namespace FastCSV
{
    /// <summary>
    /// Represents a record in a csv document.
    /// </summary>
    /// <seealso cref="FastCSV.ICsvRecord" />
    /// <seealso cref="ICloneable{CsvRecord}" />
    /// <seealso cref="IEquatable{CsvRecord}" />
    [Serializable]
    public class CsvRecord : IEnumerable<string>, ICloneable<CsvRecord>, IEquatable<CsvRecord?>
    {
        private readonly ReadOnlyArray<string> _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRecord"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="values">The values.</param>
        public CsvRecord(CsvHeader? header, IEnumerable<string> values) : this(header, values, header?.Format ?? CsvFormat.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRecord"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="values">The values.</param>
        /// <param name="format">The format.</param>
        /// <exception cref="ArgumentException">If the header csv format is different than the provided format</exception>
        public CsvRecord(CsvHeader? header, IEnumerable<string> values, CsvFormat format)
        {
            if (header != null && header.Format != format)
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
            var headerValues = CsvConverter.GetHeader<T>();
            var values = CsvConverter.GetValues(value);

            var header = new CsvHeader(headerValues, format);
            return new CsvRecord(header, values, format);
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
        /// Gets a copy of this record using the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A copy of this record with the specified format.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecord WithFormat(CsvFormat format)
        {
            if (Format == format)
            {
                return this;
            }

            return new CsvRecord(Header?.WithFormat(format), _values, format);
        }

        internal T ConvertTo<T>(CsvConverterOptions? options = null)
        {
            options ??= CsvConverterOptions.Default;
            
            if (options.IncludeHeader)
            {
                CsvHeader header = CsvHeader.FromType<T>(options);
                CsvRecord record = new CsvRecord(header, _values, options.Format);
                return CsvConverter.DeserializeFromRecord<T>(record, options);
            }
            else
            {
                return CsvConverter.DeserializeFromRecord<T>(this, options);
            }
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

                    int index = _header.IndexOf(key);
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
