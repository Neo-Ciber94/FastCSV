using FastCSV.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FastCSV.Converters
{
    /// <summary>
    /// State of a deserialization operation.
    /// </summary>
    public struct CsvDeserializeState
    {
        private readonly Either<string, CsvRecord> _source;
        private readonly Type _type;
        private int _columnIndex;

        /// <summary>
        /// Gets the options used by this state.
        /// </summary>
        public CsvConverterOptions Options { get; }

        /// <summary>
        /// Gets the record used by this state.
        /// </summary>
        public CsvRecord? Record => _source.RightOrDefault();

        /// <summary>
        /// Gets the current column index.
        /// </summary>
        public int ColumnIndex => _columnIndex;

        /// <summary>
        /// Constructs a new <see cref="CsvDeserializeState"/>.
        /// </summary>
        /// <param name="options">The csv options used for this state.</param>
        /// <param name="record">The record to get the values.</param>
        /// <param name="props">The properties.</param>
        /// <param name="columnIndex">The start index.</param>
        public CsvDeserializeState(CsvConverterOptions options, CsvRecord record, CsvPropertyInfo property, int columnIndex)
        {
            if (record.Length == 0)
            {
                throw new ArgumentException("Record must be not empty");
            }

            Options = options;
            Property = property;
            _columnIndex = columnIndex;
            _type = property.Type;
            _source = Either.FromRight(record);
        }

        /// <summary>
        /// Constructs a new <see cref="CsvDeserializeState"/>.
        /// </summary>
        /// <param name="options">The csv options used for this state.</param>
        /// <param name="type">Type of the element to convert.</param>
        /// <param name="data">The data to convert.</param>
        public CsvDeserializeState(CsvConverterOptions options, Type type, string data)
        {
            Options = options;
            Property = null;
            _columnIndex = 0;
            _type = type;
            _source = Either.FromLeft(data);
        }

        /// <summary>
        /// Gets the property for the current column.
        /// </summary>
        public CsvPropertyInfo? Property { get; }

        /// <summary>
        /// Gets the property type for the current column.
        /// </summary>
        public Type ElementType
        {
            get
            {
                Type type = Property?.Type?? _type;

                if (type.IsCollectionType())
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
            int length = _source.Fold(left: _ => 1, right: r => r.Length);

            if (_columnIndex > length)
            {
                throw new ArgumentOutOfRangeException($"Invalid column index {_columnIndex}");
            }

            ReadOnlySpan<char> result = Peek();
            _columnIndex += 1;
            return result;
        }

        /// <summary>
        /// Reads the current value.
        /// </summary>
        /// <returns>The value of the record in the current column.</returns>
        public ReadOnlySpan<char> Peek()
        {
            if (_source.IsLeft)
            {
                return _source.Left;
            }
            else
            {
                return _source.Right[_columnIndex];
            }
        }
    }
}
