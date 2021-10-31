using System;
using FastCSV.Extensions;

namespace FastCSV.Converters
{
    /// <summary>
    /// State of a deserialization operation.
    /// </summary>
    public readonly ref struct CsvDeserializeState
    {
        private readonly ReadOnlySpan<string> _values;
        private readonly ReadOnlySpan<char> _singleValue;
        private readonly bool _isSingleValue;
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
        public int Count => _isSingleValue == false? _values.Length : 1;

        /// <summary>
        /// The values to deserialize.
        /// </summary>
        public ReadOnlySpan<string> Values => _values;

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

                if (type.IsEnumerableType())
                {
                    return type.GetEnumerableElementType()!;
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
        public CsvDeserializeState(CsvConverterOptions options, CsvPropertyInfo property, ReadOnlySpan<string> values)
        {
            _values = values;
            _elementType = property.Type;
            _singleValue = default;
            _isSingleValue = false;
            Options = options;
            Property = property;
        }

        /// <summary>
        /// Constructs a new <see cref="CsvDeserializeState"/>.
        /// </summary>
        /// <param name="options">Options to be used.</param>
        /// <param name="property">Source property being deserialized.</param>
        /// <param name="value">Value to deserialize.</param>
        public CsvDeserializeState(CsvConverterOptions options, CsvPropertyInfo property, ReadOnlySpan<char> value)
        {
            _values = default;
            _elementType = property.Type;
            _singleValue = value;
            _isSingleValue = true;
            Options = options;
            Property = property;
        }

        /// <summary>
        /// Constructs a new <see cref="CsvDeserializeState"/>.
        /// </summary>
        /// <param name="options">Options to be used.</param>
        /// <param name="type">Type to deserialize to.</param>
        /// <param name="value">Value to deserialize.</param>
        public CsvDeserializeState(CsvConverterOptions options, Type type, ReadOnlySpan<char> value)
        {
            _values = default;
            _elementType = type;
            _singleValue = value;
            _isSingleValue = true;
            Options = options;
            Property = null;
        }

        /// <summary>
        /// Constructs a new <see cref="CsvDeserializeState"/>.
        /// </summary>
        /// <param name="options">Options to be used.</param>
        /// <param name="type">Type to deserialize to.</param>
        /// <param name="values">Values to deserialize.</param>
        public CsvDeserializeState(CsvConverterOptions options, Type type, ReadOnlySpan<string> values)
        {
            _values = values;
            _elementType = type;
            _singleValue = default;
            _isSingleValue = false;
            Options = options;
            Property = null;
        }

        /// <summary>
        /// Reads a <see cref="string"/> value to be deserialized.
        /// </summary>
        /// <param name="index">Index of the value to read.</param>
        /// <returns>A string value to be deserialized.</returns>
        public ReadOnlySpan<char> Read(int index = 0)
        {
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException($"Index cannot be negative or greather than {Count} but was {index}");
            }

            if (_isSingleValue)
            {
                return _singleValue;
            }
            else
            {
                return _values[index];
            }
        }
    }
}
