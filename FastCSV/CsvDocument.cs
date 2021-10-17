using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Represents an in-memory csv document.
    /// </summary>
    /// <seealso cref="ICsvDocument" />
    public partial class CsvDocument : ICsvDocument
    {
        internal readonly List<CsvRecord> _records = new List<CsvRecord>();

        public CsvDocument(IEnumerable<string> header) : this(header, CsvFormat.Default, false) { }

        public CsvDocument(IEnumerable<string> header, bool flexible) : this(header, CsvFormat.Default, flexible) { }

        public CsvDocument(IEnumerable<string> header, CsvFormat format) : this(header, format, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDocument"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> will allow records of differents lengths.</param>
        /// <exception cref="ArgumentException">Header cannot be empty</exception>
        public CsvDocument(IEnumerable<string> header, CsvFormat format, bool flexible) 
        {
            if(header.Count() == 0)
            {
                throw new ArgumentException("Header cannot be empty");
            }

            Format = format;
            IsFlexible = flexible;
            Header = new CsvHeader(header, format);
        }

        internal CsvDocument(List<CsvRecord> records, CsvHeader header, CsvFormat format, bool flexible)
        {
            if (header.Format != format)
            {
                throw new ArgumentException("Header format differs from the given format");
            }

            _records = records;
            Header = header;
            Format = format;
            IsFlexible = flexible;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public CsvHeader Header { get; }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public CsvFormat Format { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CsvDocument"/> allow records of differents lengths.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is flexible; otherwise, <c>false</c>.
        /// </value>
        public bool IsFlexible { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _records.Count == 0;

        /// <summary>
        /// Gets the number of records in this document.
        /// </summary>
        /// <value>
        /// The number of records in the document.
        /// </value>
        public int Count => _records.Count;

        /// <summary>
        /// Gets the <see cref="FastCSV.CsvRecord" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="FastCSV.CsvRecord" />.
        /// </value>
        /// <returns>The record at the specified index.</returns>
        public CsvRecord this[int index] => _records[index];

        /// <summary>
        /// Writes a new record with the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        public void Write(params object?[] values)
        {
            var array = values.Select(e => e?.ToString() ?? string.Empty);
            WriteAll(array);
        }

        /// <summary>
        /// Writes a new record with the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        public void WriteAll(IEnumerable<string> values)
        {
            int length = values.Count();

            if (!IsFlexible && length != Header.Length)
            {
                throw new ArgumentException($"Invalid number of fields, expected {Header.Length} but was {length}");
            }

            _records.Add(new CsvRecord(Header, values, Format));
        }

        /// <summary>
        /// Writes a record with the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        public void WriteValue<T>(T value)
        {
            string[] values = CsvConverter.GetValues(value!);
            WriteAll(values);
        }

        /// <summary>
        /// Writes a record with the specified value in the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="values">The values.</param>
        public void WriteAt(int index, IEnumerable<string> values)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException($"{index} > {Count}");
            }

            int length = values.Count();

            if (!IsFlexible && length != Header.Length)
            {
                throw new ArgumentException($"Invalid number of fields, expected {Header.Length} but {length} was get");
            }

            _records.Insert(index, new CsvRecord(Header, values, Format));
        }

        /// <summary>
        /// Writes a record with the specified value at the given index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void WriteValueAt<T>(int index, T value)
        {
            var values = CsvConverter.GetValues(value);
            WriteAt(index, values);
        }

        /// <summary>
        /// Writes a record using a <see cref="Builder"/>
        /// </summary>
        /// <param name="action">The action that provides the builder.</param>
        public void WriteFields(Action<Builder> action)
        {
            Builder builder = new Builder(this);
            action(builder);
            builder.WriteToDocument();
        }

        /// <summary>
        /// Updates the record at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="values">The values.</param>
        public void Update(int index, IEnumerable<string> values)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException($"{index} > {Count}");
            }

            int length = values.Count();

            if (!IsFlexible && length != Header.Length)
            {
                throw new ArgumentException($"Invalid number of fields, expected {Header.Length} but {length} was get");
            }

            _records[index] = new CsvRecord(Header, values, Format);
        }

        /// <summary>
        /// Updates the record at the specified index with the given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void UpdateValue<T>(int index, T value)
        {
            var values = CsvConverter.GetValues(value);
            Update(index, values);
        }

        /// <summary>
        /// Mutates the record at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="action">The action to mutate the record.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void MutateAt(int index, Action<CsvRecord.Mutable> action)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException($"{index} > {Count}");
            }

            _records[index] = _records[index].Mutate(action);
        }

        /// <summary>
        /// Removes the record at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The record at the specified index.</returns>
        public CsvRecord RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(index.ToString());
            }

            CsvRecord record = _records[index];
            _records.RemoveAt(index);
            return record;
        }

        /// <summary>
        /// Clears the contents of this csv.
        /// </summary>
        public void Clear()
        {
            _records.Clear();
        }

        /// <summary>
        /// Creates a copy of this document with the specified csv format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A copy using the specified format.</returns>
        public CsvDocument WithFormat(CsvFormat format)
        {
            return new CsvDocument(_records.ToList(), Header.WithFormat(format), format, IsFlexible);
        }

        /// <summary>
        /// Gets the column with the given name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The column with the given name.</returns>
        public CsvColumn GetColumn(string columnName)
        {
            return new CsvColumn(this, columnName);
        }

        /// <summary>
        /// Gets the column with the given index.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>The column with the given index.</returns>
        public CsvColumn GetColumn(int columnIndex)
        {
            return new CsvColumn(this, columnIndex);
        }

        /// <summary>
        /// Gets the columns with the specified names.
        /// </summary>
        /// <param name="columnNames">The column names.</param>
        /// <returns>The columns with the specified names or all the columns if not names is specified.</returns>
        public IEnumerable<CsvColumn> GetColumns(params string[] columnNames)
        {
            if (columnNames.Length == 0)
            {
                for(int i = 0; i < Header.Length; i++)
                {
                    yield return GetColumn(i);
                }
            }
            else
            {
                foreach (var columnName in columnNames)
                {
                    yield return GetColumn(columnName);
                }
            }
        }

        public void CopyTo(Stream destination)
        {
            CsvWriter.WriteToStream(this, Header, destination);
        }

        public Task CopyToAsync(Stream destination, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) {
                return Task.FromCanceled(cancellationToken);
            }

            CopyTo(destination);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the csv string of this document using the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(CsvFormat format)
        {
            StringBuilder sb = StringBuilderCache.Acquire();

            sb.AppendLine(Header.ToString());

            foreach (CsvRecord record in _records)
            {
                sb.AppendLine(record.ToString(format));
            }

            return StringBuilderCache.ToStringAndRelease(ref sb!);
        }

        /// <summary>
        /// Gets the csv string of this document.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(Format);
        }

        /// <summary>
        /// Gets a pretty formated string of the records of this document.
        /// </summary>
        /// <returns></returns>
        public string ToPrettyString()
        {
            return CsvUtility.ToPrettyString(_records);
        }

        /// <summary>
        /// Gets an enumerator over the records of this csv.
        /// </summary>
        /// <returns></returns>
        public List<CsvRecord>.Enumerator GetEnumerator() => _records.GetEnumerator();

        IEnumerator<CsvRecord> IEnumerable<CsvRecord>.GetEnumerator() => _records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _records.GetEnumerator();

        public struct Builder
        {
            private readonly CsvDocument _document;
            private readonly object?[] _values;
            private int _pos;

            internal Builder(CsvDocument document)
            {
                _document = document;
                _values = new object[document.Header.Length];
                _pos = 0;
            }

            public void AddField(object? value)
            {
                if(_pos == _document.Header.Length)
                {
                    throw new InvalidOperationException($"Builder cannot hold more than {_document.Header.Length} values");
                }

                _values[_pos++] = value;
            }

            public void AddField(string key, object? value)
            {
                int index = _document.Header.IndexOf(key);
                if(index >= 0)
                {
                    _values[index] = value;
                }
                else
                {
                    throw new ArgumentException($"Cannot find the key '{key}' in the header");
                }
            }

            internal void WriteToDocument()
            {
                _document.Write(_values!);
            }
        }
    }
}
