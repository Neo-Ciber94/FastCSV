using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// Provides a mechanism for store and get <see cref="ICsvValueConverter"/>.
    /// </summary>
    public interface ICsvConverterProvider
    {
        /// <summary>
        /// Gets a <see cref="ICsvValueConverter"/> for the given type.
        /// </summary>
        /// <param name="type">Type of the value to convert</param>
        /// <returns>A converter for the given type or null if not found.</returns>
        ICsvValueConverter? GetConverter(Type type);
    }
}
