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
        private static readonly IReadOnlyList<CsvPropertyInfo> s_EmptyProperties = new List<CsvPropertyInfo>();

        private readonly IReadOnlyList<CsvPropertyInfo> _props;
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

        public CsvDeserializeState(CsvConverterOptions options, CsvRecord record, IReadOnlyList<CsvPropertyInfo> props, int columnIndex)
        {
            if (props.Count == 0)
            {
                throw new ArgumentException("Expected at leasts 1 property");
            }

            if (record.Length == 0)
            {
                throw new ArgumentException("Record must be not empty");
            }

            Options = options;
            _props = props;
            _columnIndex = columnIndex;
            _type = _props.First().Type;
            _source = Either.FromRight(record);
        }

        public CsvDeserializeState(CsvConverterOptions options, Type type, string data)
        {
            Options = options;
            _props = s_EmptyProperties;
            _columnIndex = 0;
            _type = type;
            _source = Either.FromLeft(data);
        }

        /// <summary>
        /// Gets the property for the current column.
        /// </summary>
        public CsvPropertyInfo? Property
        {
            get
            {
                if (_columnIndex > _props.Count)
                {
                    return null;
                }

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
                if (_type.IsCollectionOfElements())
                {
                    return _type.GetCollectionElementType()!;
                }

                return _type;
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
