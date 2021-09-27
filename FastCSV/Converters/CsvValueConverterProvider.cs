using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// Provides a mechanism for get value converters.
    /// </summary>
    public abstract class CsvValueConverterProvider
    {
        /// <summary>
        /// The default <see cref="CsvValueConverterProvider"/>.
        /// </summary>
        public static CsvValueConverterProvider Default { get; } = new CsvDefaultValueConverterProvider();

        /// <summary>
        /// Gets a <see cref="ICsvValueConverter"/> for the given type or null if any can be found.
        /// </summary>
        /// <param name="type">Type of the elements the converter can convert.</param>
        /// <returns>A converter for the given type or null if not found.</returns>
        public abstract ICsvValueConverter? GetConverter(Type type);
    }
}
