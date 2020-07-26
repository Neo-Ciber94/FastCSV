using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Represents an in-memory csv document.
    /// </summary>
    /// <seealso cref="ICsvDocument" />
    public class CsvDocument : ICsvDocument
    {
        private readonly List<CsvRecord> _records = new List<CsvRecord>();

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

        private CsvDocument(List<CsvRecord> records, CsvHeader header, CsvFormat format, bool flexible)
        {
            _records = records;
            Header = header;
            Format = format;
            IsFlexible = flexible;
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified string.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <returns></returns>
        public static CsvDocument FromCsv(string csv)
        {
            return FromCsv(csv, CsvFormat.Default, false);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified string.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="flexible">if set to <c>true</c> [flexible].</param>
        /// <returns></returns>
        public static CsvDocument FromCsv(string csv, bool flexible)
        {
            return FromCsv(csv, CsvFormat.Default, flexible);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified string.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static CsvDocument FromCsv(string csv, CsvFormat format)
        {
            return FromCsv(csv, format, false);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified string.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> will allow records of differents lengths.</param>
        /// <returns></returns>
        public static CsvDocument FromCsv(string csv, CsvFormat format, bool flexible)
        {
            if (csv.IsNullOrBlank())
            {
                throw new ArgumentException("CSV is empty");
            }

            using MemoryStream memory = new MemoryStream(csv.Length);
            using (StreamWriter writer = new StreamWriter(memory, leaveOpen: true))
            {
                writer.Write(csv);
                writer.Flush();
                memory.Position = 0;
            }

            using (CsvReader reader = new CsvReader(new StreamReader(memory), format))
            {
                List<CsvRecord>? records;

                if (flexible)
                {
                    records = reader.ReadAll().ToList();
                }
                else
                {
                    records = new List<CsvRecord>();
                    int headerLength = reader.Header!.Length;

                    foreach (var r in reader.ReadAll())
                    {
                        int recordLength = r.Length;
                        if (recordLength != headerLength)
                        {
                            throw new InvalidOperationException($"Invalid record length for non-flexible csv, " +
                                $"expected {headerLength} but {recordLength} was get");
                        }

                        records.Add(r);
                    }
                }

                return new CsvDocument(records, reader.Header!, format, flexible);
            }
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
        /// Gets a value indicating whether this instance is flexible.
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
        public void Write(params object[] values)
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
                throw new ArgumentException($"Invalid number of fields, expected {Header.Length} but {length} was get");
            }

            _records.Add(new CsvRecord(Header, values, Format));
        }

        /// <summary>
        /// Writes a record with the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        public void WriteWith<T>(T value)
        {
            List<string> values = CsvUtility.GetValues(value);
            WriteAll(values);
        }

        /// <summary>
        /// Writes a record with the specified value in the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="values">The values.</param>
        public void WriteAt(int index, IEnumerable<string> values)
        {
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException($"{index} > {Count}");
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
        public void WriteAtWith<T>(int index, T value)
        {
            var values = CsvUtility.GetValues(value);
            WriteAt(index, values);
        }

        /// <summary>
        /// Updates the record at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="values">The values.</param>
        public void Update(int index, IEnumerable<string> values)
        {
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException($"{index} > {Count}");
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
        public void UpdateWith<T>(int index, T value)
        {
            var values = CsvUtility.GetValues(value);
            Update(index, values);
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
        /// Gets the csv string of this document using the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(CsvFormat format)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Header.ToString());

            foreach (CsvRecord record in _records)
            {
                sb.AppendLine(record.ToString(format));
            }

            return sb.ToString();
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
        /// Gets an enumerator over the records of this csv.
        /// </summary>
        /// <returns></returns>
        public List<CsvRecord>.Enumerator GetEnumerator() => _records.GetEnumerator();

        IEnumerator<CsvRecord> IEnumerable<CsvRecord>.GetEnumerator() => _records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _records.GetEnumerator();
    }
}
