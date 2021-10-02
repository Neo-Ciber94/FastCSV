using FastCSV.Utils;
using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// State of a deserialization operation.
    /// </summary>
    public readonly struct CsvDeserializeState
    {
        private readonly ReadOnlyMemory<string> _values;
        private readonly string? _singleValue;
        private readonly Type _elementType;

        /// <summary>
        /// Options for the deserialization operation.
        /// </summary>
        public CsvConverterOptions Options { get; }

        /// <summary>
        /// Property being deserialized.
        /// </summary>
        public CsvPropertyInfo? Property { get; }

        /// <summary>
        /// Number of values being deserialized.
        /// </summary>
        public int Count => _singleValue == null ? _values.Length : 1;

        /// <summary>
        /// Type of the elements being deserialized.
        /// 
        /// <para>
        /// If the element is an array this will return the type of the array element.
        /// </para>
        /// </summary>
        public Type ElementType
        {
            get
            {
                Type type = Property?.Type ?? _elementType;

                if (type.IsCollectionType())
                {
                    return type.GetCollectionElementType()!;
                }

                return type;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="CsvDeserializeState"/>.
        /// </summary>
        /// <param name="options">Options to be used.</param>
        /// <param name="property">Source property being deserialized.</param>
        /// <param name="values">Values to deserialize.</param>
        public CsvDeserializeState(CsvConverterOptions options, CsvPropertyInfo property, ReadOnlyMemory<string> values)
        {
            _values = values;
            _elementType = property.Type;
            _singleValue = null;
            Options = options;
            Property = property;
        }

        /// <summary>
        /// Constructs a new <see cref="CsvDeserializeState"/>.
        /// </summary>
        /// <param name="options">Options to be used.</param>
        /// <param name="property">Source property being deserialized.</param>
        /// <param name="value">Value to deserialize.</param>
        public CsvDeserializeState(CsvConverterOptions options, CsvPropertyInfo property, string value)
        {
            _values = default;
            _elementType = property.Type;
            _singleValue = value;
            Options = options;
            Property = property;
        }

        /// <summary>
        /// Constructs a new <see cref="CsvDeserializeState"/>.
        /// </summary>
        /// <param name="options">Options to be used.</param>
        /// <param name="type">Type to deserialize to.</param>
        /// <param name="value">Value to deserialize.</param>
        public CsvDeserializeState(CsvConverterOptions options, Type type, string value)
        {
            _values = default;
            _elementType = type;
            _singleValue = value;
            Options = options;
            Property = null;
        }

        /// <summary>
        /// Reads a <see cref="string"/> value to be deserialized.
        /// </summary>
        /// <param name="index">Index of the value to read.</param>
        /// <returns>A string value to be deserialized.</returns>
        public string Read(int index = 0)
        {
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException($"Index cannot be negative or greather than {Count} but was {index}");
            }

            if (_singleValue != null)
            {
                return _singleValue;
            }
            else
            {
                return _values.Span[index];
            }
        }
    }
}
