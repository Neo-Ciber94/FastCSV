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

        private readonly ICsvValueConverter _arrayConverter = new CsvArrayConverter();

        public DefaultCsvCollectionConverterProvider()
        {
            // Initialize the _converters   
        }

        public override ICsvValueConverter? GetConverter(Type collectionType)
        {
            if (collectionType.IsArray)
            {
                return _arrayConverter;
            }

            if (_converters.TryGetValue(collectionType, out ICsvValueConverter? collectionConverter))
            {
                return collectionConverter;
            }

            return null;
        }
    }
}
