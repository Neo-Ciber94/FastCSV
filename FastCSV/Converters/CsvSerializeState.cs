using FastCSV.Collections;

namespace FastCSV.Converters
{
    /// <summary>
    /// State of a serialization operation.
    /// </summary>
    public struct CsvSerializeState
    {
        private readonly ValueOrList<string> _serialized;

        /// <summary>
        /// Options used for serialization.
        /// </summary>
        public CsvConverterOptions Options { get; }

        /// <summary>
        /// Gets a readonly list of the current serialized values.
        /// </summary>
        public ReadOnlyValueOrList<string> Serialized => _serialized;

        public CsvSerializeState(CsvConverterOptions options)
        {
            Options = options;;
            _serialized = ValueOrList<string>.Empty;
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
