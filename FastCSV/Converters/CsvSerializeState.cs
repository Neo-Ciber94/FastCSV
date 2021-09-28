using FastCSV.Collections;
using System.Collections.Generic;

namespace FastCSV.Converters
{
    /// <summary>
    /// State of a serialization operation.
    /// </summary>
    public struct CsvSerializeState
    {
        internal readonly List<string> _serialized;

        /// <summary>
        /// Options used for serialization.
        /// </summary>
        public CsvConverterOptions Options { get; }

        /// <summary>
        /// Gets a readonly list of the current serialized values.
        /// </summary>
        public IReadOnlyList<string> Serialized => _serialized;

        public CsvSerializeState(CsvConverterOptions options, int capacity = 0)
        {
            Options = options;;
            _serialized = new List<string>(capacity);
        }

        /// <summary>
        /// Write the serialized value to this state.
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            _serialized.Add(value);
        }

        /// <summary>
        /// Writes a null value.
        /// </summary>
        public void WriteNull()
        {
            _serialized.Add(CsvConverter.Null);
        }
    }
}
