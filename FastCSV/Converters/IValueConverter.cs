using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// Provides a mechanism for converts a value from and to <see cref="string"/>.
    /// </summary>
    public interface IValueConverter : ICsvValueConverter
    {
        /// <summary>
        /// Attempts to parse a <see cref="string"/> to a specify value.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="value">The resulting value.</param>
        /// <returns><c>true</c> if the value can be parse, otherwise <c>false</c>.</returns>
        public bool TryParse(ReadOnlySpan<char> s, out object? value);

        /// <summary>
        /// Reads the value as <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the value.</returns>
        public string? Read(object? value);

        bool ICsvValueConverter.TryDeserialize(out object? result, Type elementType, ref CsvDeserializeState state)
        {
            return TryParse(state.Read(), out result);
        }

        bool ICsvValueConverter.TrySerialize(object? value, Type elementType, ref CsvSerializeState state)
        {
            string? result = Read(value);
            if (result == null)
            {
                return false;
            }

            state.Write(result);
            return true;
        }
    }

    /// <summary>
    /// Provides a mechanism for converts a typed value from and to <see cref="string"/>.
    /// </summary>
    /// <typeparam name="T">The value to convert.</typeparam>
    public interface IValueConverter<T> : IValueConverter
    {
        /// <summary>
        /// Attempts to parse a <see cref="string"/> to a value of type <see cref="T"/>.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="value">The resulting value.</param>
        /// <returns><c>true</c> if the value can be parse, otherwise <c>false</c>.</returns>
        public bool TryParse(ReadOnlySpan<char> s, out T value);

        /// <summary>
        /// Reads the value as <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the value.</returns>
        public string? Read(T value);

        /// <inheritdoc/>
        bool ICsvValueConverter.CanConvert(Type type)
        {
            return typeof(T) == type;
        }

        /// <inheritdoc/>
        string? IValueConverter.Read(object? value)
        {
            return Read((T)value!);
        }

        /// <inheritdoc/>
        bool IValueConverter.TryParse(ReadOnlySpan<char> s, out object? value)
        {
            value = null;

            if (TryParse(s, out T result))
            {
                value = result;
            }

            return value != null;
        }
    }
}
