using System;
using System.Collections.Generic;

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
        public static CsvValueConverterProvider Default { get; } = new DefaultCsvValueConverterProvider();

        /// <summary>
        /// Gets a <see cref="ICsvValueConverter"/> for the given type or null if any can be found.
        /// </summary>
        /// <param name="elementType">Type of the elements the converter can convert.</param>
        /// <returns>A converter for the given type or null if not found.</returns>
        public abstract ICsvValueConverter? GetConverter(Type elementType);
    }

    internal class DefaultCsvValueConverterProvider : CsvValueConverterProvider
    {
        private readonly Dictionary<Type, ICsvValueConverter> _converters = new Dictionary<Type, ICsvValueConverter>();

        public DefaultCsvValueConverterProvider()
        {
            // Initialize converters
        }

        public override ICsvValueConverter? GetConverter(Type elementType)
        {
            if (_converters.TryGetValue(elementType, out ICsvValueConverter? converter))
            {
                return converter;
            }

            return null;
        }
    }
}
