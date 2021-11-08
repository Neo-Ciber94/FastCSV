using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FastCSV.Converters;
using FastCSV.Internal;

namespace FastCSV
{
    /// <summary>
    /// Represents a column in a CSV file.
    /// </summary>
    /// <seealso cref="IEnumerable{string}" />
    public readonly struct CsvColumn : IEnumerable<string>
    {
        private readonly IEnumerable<CsvRecord> _records;
        private readonly int _columnIndex;

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumn"/> struct.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <exception cref="ArgumentException">If cannot find the column.</exception>
        public CsvColumn(IEnumerable<CsvRecord> records, string columnName)
        {
            CsvHeader? header = records.FirstOrDefault()?.Header;

            if (header == null)
            {
                throw new ArgumentException("No header found");
            }

            int index = header.IndexOf(columnName);

            if (index < 0)
            {
                throw new ArgumentException("Cannot find a column named: " + columnName);
            }

            _records = records;
            _columnIndex = index;
            Name = columnName;
        }

        public CsvColumn(IEnumerable<CsvRecord> records, int columnIndex)
        {
            CsvHeader? header = records.FirstOrDefault()?.Header;

            if (header == null)
            {
                throw new ArgumentException("No header found");
            }

            if (columnIndex < 0 || columnIndex >= header.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex), $"{nameof(columnIndex)}: {columnIndex}");
            }

            _records = records;
            _columnIndex = columnIndex;
            Name = header[columnIndex]!;
        }

        public IEnumerable<T> Cast<T>(CsvConverterOptions? options = null)
        {
            options ??= CsvConverterOptions.Default;
            Type type = typeof(T);
            var converter = CsvConverter.GetConverter(type, options);

            if (converter == null)
            {
                return Array.Empty<T>();
            }

            List<T> result = new();

            foreach (string value in this)
            {
                var state = new CsvDeserializeState(options, type, value);

                if (!converter.TryDeserialize(out object? obj, type, ref state))
                {
                    throw ThrowHelper.CannotDeserializeToType(new string[] { value }, type);
                }

                result.Add((T)obj!);
            }

            return result;
        }

        /// <summary>
        /// Gets an enumerator over the values of this column.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator() => new(this);

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IEnumerator<string>
        {
            private readonly CsvColumn _column;
            private IEnumerator<CsvRecord> _enumerator;
            private CsvRecord? _current;

            public Enumerator(CsvColumn column)
            {
                _enumerator = column._records.GetEnumerator();
                _column = column;
                _current = null;
            }

            public string Current => _current?[_column._columnIndex]!;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (!_enumerator.MoveNext())
                {
                    return false;
                }

                _current = _enumerator.Current;
                AssertIsValidColumnRecord(_current, _column._columnIndex, _column.Name);
                return true;
            }

            public void Reset()
            {
                _enumerator = _column._records.GetEnumerator();
            }

            void IDisposable.Dispose()
            {
            }
        }

        internal static void AssertIsValidColumnRecord(CsvRecord record, int columnIndex, string columnName)
        {
            if (record.Header == null)
            {
                throw new ArgumentException("No header found");
            }

            if (columnIndex < 0 || columnIndex >= record.Header.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex), $"{nameof(columnIndex)}: {columnIndex}");
            }

            if (columnName != record.Header[columnIndex])
            {
                throw new ArgumentException($"{nameof(columnName)}: {columnName}", nameof(columnName));
            }
        }
    }
}
