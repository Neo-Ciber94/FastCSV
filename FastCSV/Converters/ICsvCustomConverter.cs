using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// A simplified version of the <see cref="ICsvValueConverter"/> to convert string from and to objects.
    /// </summary>
    public interface ICsvCustomConverter : ICsvValueConverter
    {
        /// <summary>
        /// Attempts to parse a <see cref="string"/> to a specify value.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="value">The resulting value.</param>
        /// <returns><c>true</c> if the value can be parse, otherwise <c>false</c>.</returns>
        public bool ConvertTo(ReadOnlySpan<char> s, out object? value);

        /// <summary>
        /// Reads the value as <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the value.</returns>
        public string? ConvertFrom(object? value);

        bool ICsvValueConverter.TryDeserialize(out object? result, Type elementType, ref CsvDeserializeState state)
        {
            return ConvertTo(state.Read(), out result);
        }

        bool ICsvValueConverter.TrySerialize(object? value, Type elementType, ref CsvSerializeState state)
        {
            string? result = ConvertFrom(value);
            if (result == null)
            {
                return false;
            }

            state.Write(result);
            return true;
        }
    }

    /// <summary>
    /// A simplified version of the <see cref="ICsvValueConverter{T}"/> to convert string from and to objects.
    /// </summary>
    /// <typeparam name="T">The value to convert.</typeparam>
    public interface ICsvCustomConverter<T> : ICsvCustomConverter
    {
        /// <summary>
        /// Attempts to parse a <see cref="string"/> to a value of type <see cref="T"/>.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="value">The resulting value.</param>
        /// <returns><c>true</c> if the value can be parse, otherwise <c>false</c>.</returns>
        public bool ConvertTo(ReadOnlySpan<char> s, out T value);

        /// <summary>
        /// Reads the value as <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the value.</returns>
        public string? ConvertFrom(T value);

        /// <inheritdoc/>
        bool ICsvValueConverter.CanConvert(Type type)
        {
            return typeof(T) == type;
        }

        /// <inheritdoc/>
        string? ICsvCustomConverter.ConvertFrom(object? value)
        {
            return ConvertFrom((T)value!);
        }

        /// <inheritdoc/>
        bool ICsvCustomConverter.ConvertTo(ReadOnlySpan<char> s, out object? value)
        {
            value = null;

            if (ConvertTo(s, out T result))
            {
                value = result;
            }

            return value != null;
        }
    }
}
