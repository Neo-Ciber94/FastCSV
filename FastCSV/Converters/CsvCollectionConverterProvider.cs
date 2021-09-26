using System;
using System.Collections.Generic;

namespace FastCSV.Converters
{
    /// <summary>
    /// Provides a mechanism for get collection converters.
    /// </summary>
    public abstract class CsvCollectionConverterProvider
    {
        /// <summary>
        /// Gets an <see cref="CsvCollectionConverter{TCollection, TElement}"/> for the given type.
        /// </summary>
        /// <param name="collectionType">The type of the collection.</param>
        /// <returns>The collection converter or null.</returns>
        public abstract ICsvValueConverter? GetCollectionConverter(Type collectionType);
    }

    internal class DefaultCsvCollectionConverterProvider : CsvCollectionConverterProvider
    {
        private readonly IDictionary<Type, ICsvValueConverter> _converters = new Dictionary<Type, ICsvValueConverter>();

        public DefaultCsvCollectionConverterProvider()
        {
            // Initialize the _converters
        }

        public override ICsvValueConverter? GetCollectionConverter(Type collectionType)
        {
            if (_converters.TryGetValue(collectionType, out ICsvValueConverter? collectionConverter))
            {
                return collectionConverter;
            }

            return null;
        }
    }
}
