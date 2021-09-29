using FastCSV.Collections;
using System;
using System.Collections.Generic;

namespace FastCSV.Converters
{
    /// <summary>
    /// State of a serialization operation.
    /// </summary>
    public struct CsvSerializeState
    {
        private readonly List<string> _serialized;

        /// <summary>
        /// Options used for serialization.
        /// </summary>
        public CsvConverterOptions Options { get; }

        /// <summary>
        /// Provider for <see cref="ICsvValueConverter"/>.
        /// </summary>
        public CsvValueConverterProvider Provider { get; }

        /// <summary>
        /// Gets sets the converter for the current state.
        /// </summary>
        public ICsvValueConverter? Converter { get; set; }

        /// <summary>
        /// Gets a readonly list of the current serialized values.
        /// </summary>
        public IReadOnlyList<string> Serialized => _serialized;

        public CsvSerializeState(CsvConverterOptions options, CsvValueConverterProvider? converterProvider = null, int capacity = 0)
        {
            Options = options;
            Provider = converterProvider ?? CsvValueConverterProvider.Default;
            Converter = null;
            _serialized = new List<string>(capacity);
        }

        /// <summary>
        /// Gets a converter for the given type.
        /// </summary>
        /// <param name="elementType">The element type.</param>
        /// <returns>A converter for the given type or null.</returns>
        public ICsvValueConverter? GetConverter(Type elementType)
        {
            return CsvConverter.GetConverter(elementType, Options, Converter);
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
