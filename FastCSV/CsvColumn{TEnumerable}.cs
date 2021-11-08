
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FastCSV.Collections;
using FastCSV.Converters;
using FastCSV.Internal;

namespace FastCSV
{
    public readonly struct CsvColumn<TEnumerable, TEnumerator> 
        : IValueEnumerable<string, CsvColumn<TEnumerable, TEnumerator>.Enumerator>
        where TEnumerable : IValueEnumerable<CsvRecord, TEnumerator>
        where TEnumerator : IEnumerator<CsvRecord>
    {
        private readonly TEnumerable _records;
        private readonly int _columnIndex;

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        public CsvColumn(TEnumerable records, string columnName)
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

        public CsvColumn(TEnumerable records, int columnIndex)
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

        /// <summary>
        /// Transforms the columns to the type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="options">The options to use.</param>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>An enumerable of object of type T.</returns>
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

        public static implicit operator CsvColumn(CsvColumn<TEnumerable, TEnumerator> column)
        {
            return new CsvColumn(column._records, column._columnIndex);
        }

        public Enumerator GetEnumerator() => new(this);
        
        public struct Enumerator : IEnumerator<string>
        {
            private readonly CsvColumn<TEnumerable, TEnumerator> _column;
            private TEnumerator _enumerator;
            private CsvRecord? _current;

            public Enumerator(CsvColumn<TEnumerable, TEnumerator> column)
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
                CsvColumn.AssertIsValidColumnRecord(_current, _column._columnIndex, _column.Name);
                return true;
            }

            public void Reset()
            {
                _enumerator = _column._records.GetEnumerator();
            }

            void IDisposable.Dispose() { }
        }
    }
}