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
        private readonly IReadOnlyList<CsvField> _fields;
        private int _columnIndex;

        public CsvConverterOptions Options { get; }

        public CsvRecord Record { get; }

        public int ColumnIndex => _columnIndex;

        internal CsvDeserializeState(CsvConverterOptions options, CsvRecord record, IReadOnlyList<CsvField> fields, int columnIndex)
        {
            Options = options;
            Record = record;
            _fields = fields;
            _columnIndex = columnIndex;
        }

        public Type ElementType
        {
            get
            {
                var type = _fields[_columnIndex].Type;

                if (type.IsCollectionOfElements())
                {
                    return type.GetCollectionElementType()!;
                }

                return type;
            }
        }

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
