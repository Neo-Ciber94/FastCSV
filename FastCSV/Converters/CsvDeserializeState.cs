using FastCSV.Utils;
using System;
using System.Collections.Generic;

namespace FastCSV.Converters
{
    /// <summary>
    /// State of a deserialization operation.
    /// </summary>
    public struct CsvDeserializeState
    {
        private readonly IReadOnlyList<CsvPropertyInfo> _props;
        private int _columnIndex;

        /// <summary>
        /// Gets the options used by this state.
        /// </summary>
        public CsvConverterOptions Options { get; }

        /// <summary>
        /// Gets the record used by this state.
        /// </summary>
        public CsvRecord Record { get; }

        /// <summary>
        /// Gets the current column index.
        /// </summary>
        public int ColumnIndex => _columnIndex;

        public CsvDeserializeState(CsvConverterOptions options, CsvRecord record, IReadOnlyList<CsvPropertyInfo> props, int columnIndex)
        {
            Options = options;
            Record = record;
            _props = props;
            _columnIndex = columnIndex;
        }

        /// <summary>
        /// Gets the property for the current column.
        /// </summary>
        public CsvPropertyInfo CurrentProperty
        {
            get
            {
                return _props[_columnIndex];
            }
        }

        /// <summary>
        /// Gets the property type for the current column.
        /// </summary>
        public Type CurrentElementType
        {
            get
            {
                var type = CurrentProperty.Type;

                if (type.IsCollectionOfElements())
                {
                    return type.GetCollectionElementType()!;
                }

                return type;
            }
        }

        /// <summary>
        /// Reads the current value and advance to the next column.
        /// </summary>
        /// <returns>The value of the record in the current column.</returns>
        public ReadOnlySpan<char> Read()
        {
            if (_columnIndex >= Record.Length)
            {
                throw new ArgumentOutOfRangeException($"Invalid record index {_columnIndex}");
            }

            return Record[++_columnIndex];
        }
    }
}
