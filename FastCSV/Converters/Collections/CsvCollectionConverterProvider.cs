using FastCSV.Collections;
using FastCSV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters.Collections
{
    internal class CsvCollectionConverterProvider : ICsvConverterProvider
    {
        public static readonly CsvCollectionConverterProvider Default = new CsvCollectionConverterProvider();

        private readonly IDictionary<Type, ICsvValueConverter> _converters = new Dictionary<Type, ICsvValueConverter>();

        // Array
        private ArrayConverter? _arrayConverter = null;
        private ArrayConverter GetOrCreateArrayConverter()
        {
            if (_arrayConverter == null)
            {
                _arrayConverter = new ArrayConverter();
            }

            return _arrayConverter;
        }

        // IList
        private IListConverter? _listConverter = null;
        private IListConverter GetOrCreateIListConverter()
        {
            if (_listConverter == null)
            {
                _listConverter = new IListConverter();
            }

            return _listConverter;
        }

        public ICsvValueConverter? GetConverter(Type type)
        {
            if (type.IsArray)
            {
                return GetOrCreateArrayConverter();
            }

            if (_converters.TryGetValue(type, out ICsvValueConverter? collectionConverter))
            {
                return collectionConverter;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = type.GetCollectionElementType()!;
                var listOfTConverter = GenericConverterFactory.Create(typeof(IListOfTConverter<>), elementType);
                _converters.Add(type, listOfTConverter);
                return listOfTConverter;
            }

            // Fallback
            return GetNonGenericCollectionConverter(type);
        }

        private ICsvValueConverter? GetNonGenericCollectionConverter(Type type)
        {
            if (typeof(IList) == type)
            {
                return GetOrCreateIListConverter();
            }

            if (typeof(ICollection) == type)
            {
                return GetOrCreateIListConverter();
            }

            return null;
        }
    }
}
