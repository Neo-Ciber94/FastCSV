
using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// Provides a mechanism for converts a value from and to <see cref="string"/>.
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Checks whether this converter can parse values from the given type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if can parse a value from the given type, otherwise <c>false</c>.</returns>
        public bool CanConvert(Type type);

        /// <summary>
        /// Attempts to parse a <see cref="string"/> to a specify value.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="value">The resulting value.</param>
        /// <returns><c>true</c> if the value can be parse, otherwise <c>false</c>.</returns>
        public bool TryParse(string? s, out object? value);

        /// <summary>
        /// Reads the value as <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the value.</returns>
        public string? Read(object? value);
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
        public bool TryParse(string? s, out T value);

        /// <summary>
        /// Reads the value as <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A string representation of the value.</returns>
        public string? Read(T value);

        /// <inheritdoc/>
        bool IValueConverter.CanConvert(Type type)
        {
            return typeof(T) == type;
        }

        /// <inheritdoc/>
        string? IValueConverter.Read(object? value)
        {
            return Read((T)value!);
        }

        /// <inheritdoc/>
        bool IValueConverter.TryParse(string? s, out object? value)
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
