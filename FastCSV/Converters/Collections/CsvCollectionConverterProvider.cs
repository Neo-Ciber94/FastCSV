using System;
using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    /// <summary>
    /// Provides a mechanism for get collection converters.
    /// </summary>
    public abstract class CsvCollectionConverterProvider : CsvValueConverterProvider
    {
        public static CsvCollectionConverterProvider Collections { get; } = new DefaultCsvCollectionConverterProvider();
    }

    internal class DefaultCsvCollectionConverterProvider : CsvCollectionConverterProvider
    {
        private readonly IDictionary<Type, ICsvValueConverter> _converters = new Dictionary<Type, ICsvValueConverter>();

        public DefaultCsvCollectionConverterProvider()
        {
            // Initialize the _converters   
        }

        public override ICsvValueConverter? GetConverter(Type collectionType)
        {
            if (_converters.TryGetValue(collectionType, out ICsvValueConverter? collectionConverter))
            {
                return collectionConverter;
            }

            return null;
        }
    }
}
