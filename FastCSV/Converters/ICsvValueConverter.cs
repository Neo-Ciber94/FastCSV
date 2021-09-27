using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// Provides a mechanism for serializing and deserializing a value.
    /// </summary>
    public interface ICsvValueConverter
    {
        /// <summary>
        /// Deserializes a value of the given type.
        /// </summary>
        /// <param name="result">The resulting value.</param>
        /// <param name="elementType">The type of the value to serialize.</param>
        /// <param name="state">The state of the deserialization.</param>
        /// <returns><c>true</c> if the deserialization was correct.</returns>
        public bool TryDeserialize(out object? result, Type elementType, ref CsvDeserializeState state);

        /// <summary>
        /// Serialize the value into an array to <see cref="string"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="elementType">The type of the value to serialize.</param>
        /// <param name="state">The state of the serialization.</param>
        public bool TrySerialize(object? value, Type elementType, ref CsvSerializeState state);

        /// <summary>
        /// Checks if this instance can convert a value of the given type.
        /// </summary>
        /// <param name="type">Type of the value to serialize.</param>
        /// <returns><c>true</c> if the value can be converted otherwise false.</returns>
        public bool CanConvert(Type type);
    }

    /// <summary>
    /// Provides a mechanism for serializing and deserializing a value of type <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the value to convert.</typeparam>
    public interface ICsvValueConverter<T> : ICsvValueConverter
    {
        /// <summary>
        /// Deserializes a value of type <see cref="T"/>.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="state">The state of the serialization.</param>
        public bool TrySerialize(T value, ref CsvSerializeState state);

        /// <summary>
        /// Serialize the value into an array to <see cref="string"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="state">The state of the serialization.</param>
        public bool TryDeserialize(out T value, ref CsvDeserializeState state);

        /// <inheritdoc/>
        bool ICsvValueConverter.CanConvert(Type type)
        {
            return typeof(T) == type;
        }

        /// <inheritdoc/>
        bool ICsvValueConverter.TrySerialize(object? value, Type elementType, ref CsvSerializeState state)
        {
            return TrySerialize((T)value!, ref state);
        }

        /// <inheritdoc/>
        bool ICsvValueConverter.TryDeserialize(out object? result, Type elementType, ref CsvDeserializeState state)
        {
            result = default;

            if (TryDeserialize(out T value, ref state))
            {
                result = value!;
                return true;
            }

            return false;
        }
    }
}
