using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV
{
    /// <summary>
    /// A view to a column of csv data.
    /// </summary>
    /// <seealso cref="IEnumerable{string}" />
    public readonly struct CsvColumn : IEnumerable<string>
    {
        private readonly IReadOnlyList<CsvRecord> _records;
        private readonly int _columnIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumn"/> struct.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <exception cref="ArgumentException">If cannot find the column.</exception>
        public CsvColumn(CsvDocument document, string columnName)
        {
            _columnIndex = document.Header.IndexOf(columnName);

            if(_columnIndex < 0)
            {
                throw new ArgumentException("Cannot find a column named: " + columnName);
            }

            _records = document._records;
            Name = columnName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumn"/> struct.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public CsvColumn(CsvDocument document, int columnIndex)
        {
            _columnIndex = columnIndex;
            if(_columnIndex < 0 | columnIndex > document.Header.Length)
            {
                throw new ArgumentOutOfRangeException($"{nameof(columnIndex)}: {columnIndex}");
            }

            _records = document._records;
            Name = document.Header[_columnIndex];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvColumn"/> struct.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <exception cref="ArgumentException">
        /// If the first record don't have a header.
        /// </exception>
        public CsvColumn(IReadOnlyList<CsvRecord> records, int columnIndex)
        {
            if (records.Count == 0)
            {
                throw new ArgumentException($"{nameof(records)} is empty");
            }

            CsvHeader? header = records[0].Header;

            if (header == null)
            {
                throw new ArgumentException("First record don't have a header.");
            }

            _records = records;
            _columnIndex = columnIndex;
            Name = header[columnIndex]!;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets an enumerator over the values of this column.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator() => new Enumerator(this);

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IEnumerator<string>
        {
            private readonly IReadOnlyList<CsvRecord> _records;
            private readonly int _columnIndex;
            private int _index;

            internal Enumerator(in CsvColumn column)
            {
                _records = column._records;
                _columnIndex = column._columnIndex;
                _index = -1;
            }

            public string Current => _records[_index][_columnIndex];

            object? IEnumerator.Current => Current;

            public bool MoveNext()
            {
                int i = _index + 1;

                if(i < _records.Count)
                {
                    _index = i;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose() { }
        }
    }
}
