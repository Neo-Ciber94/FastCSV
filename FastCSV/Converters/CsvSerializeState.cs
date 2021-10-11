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
        private readonly List<string> _buffer;
        private readonly object? _value;

        /// <summary>
        /// Options used for serialization.
        /// </summary>
        public CsvConverterOptions Options { get; }

        /// <summary>
        /// Gets sets the converter for the current state.
        /// </summary>
        public ICsvValueConverter? Converter { get; }

        /// <summary>
        /// Gets a readonly list of the current serialized values.
        /// </summary>
        public IReadOnlyList<string> Serialized => _buffer;

        /// <summary>
        /// The value being serialized.
        /// </summary>
        public object? Value => _value;

        public CsvSerializeState(CsvConverterOptions options, object? value, List<string> buffer, ICsvValueConverter? converter = null)
        {
            Options = options;
            Converter = converter;
            _value = value;
            _buffer = buffer;
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
            _buffer.Add(value);
        }

        /// <summary>
        /// Writes a null value.
        /// </summary>
        public void WriteNull()
        {
            _buffer.Add(CsvConverter.Null);
        }
    }
}
