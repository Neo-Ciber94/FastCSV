using System;
using System.Collections;
using System.Collections.Generic;
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

        public CsvHeader Header { get; }

        public CsvFormat Format { get; }

        public bool IsFlexible { get; }

        public bool IsEmpty => _records.Count == 0;

        public int Count => _records.Count;

        public CsvRecord this[int index] => _records[index];

        public void Write(params object[] values)
        {
            var array = values.Select(e => e?.ToString() ?? string.Empty);
            WriteAll(array);
        }

        public void WriteAll(IEnumerable<string> values)
        {
            int length = values.Count();

            if (!IsFlexible && length != Header.Length)
            {
                throw new ArgumentException($"Invalid number of fields, expected {Header.Length} but {length} was get");
            }

            _records.Add(new CsvRecord(Header, values, Format));
        }

        public void WriteWith<T>(T value)
        {
            List<string> values = CsvUtility.GetValues(value);
            WriteAll(values);
        }

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

        public void WriteAtWith<T>(int index, T value)
        {
            var values = CsvUtility.GetValues(value);
            WriteAt(index, values);
        }

        public void Update(int index, IEnumerable<string> values)
        {
            if(index < 0 || index > Count)
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

        public void UpdateWith<T>(int index, T value)
        {
            var values = CsvUtility.GetValues(value);
            Update(index, values);
        }

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

        public void Clear()
        {
            _records.Clear();
        }

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

        public override string ToString()
        {
            return ToString(Format);
        }

        public List<CsvRecord>.Enumerator GetEnumerator() => _records.GetEnumerator();

        IEnumerator<CsvRecord> IEnumerable<CsvRecord>.GetEnumerator() => _records.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _records.GetEnumerator();
    }
}
