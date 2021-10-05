using FastCSV.Collections;
using FastCSV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        private ArrayListConverter? _listConverter = null;
        private ArrayListConverter GetOrCreateIListConverter()
        {
            if (_listConverter == null)
            {
                _listConverter = new ArrayListConverter();
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

            if (type.IsGenericType)
            {
                /*
                 * List<T> converter is used for serialize/deserialize the following types:
                 * - List<T>
                 * - IList<T>
                 * - IReadOnlyList<T>
                 * - ICollection<T>
                 * - IReadOnlyCollection<T>
                 * - IEnumerable<T>
                 */
                if (CanAssignToType(typeof(List<>), type))
                {
                    var elementType = type.GetCollectionElementType()!;
                    var listOfTConverter = GenericConverterFactory.CreateCollectionConverter(typeof(ListOfTConverter<>), elementType);
                    _converters.Add(type, listOfTConverter);
                    return listOfTConverter;
                }
            }

            // Fallback
            return GetNonGenericCollectionConverter(type);
        }

        private ICsvValueConverter? GetNonGenericCollectionConverter(Type type)
        {
            /*
             * ArrayList converter is used for serialize/deserialize the following types:
             * - ArrayList
             * - IList
             * - ICollection
             * - IEnumerable
             */
            if (typeof(ArrayList).IsAssignableFrom(type))
            {
                return GetOrCreateIListConverter();
            }

            return null;
        }

        private static bool CanAssignToType(Type collectionGenericDefinition, Type collectionType)
        {
            Debug.Assert(collectionGenericDefinition.IsGenericTypeDefinition);
            Debug.Assert(collectionType.IsCollectionType());

            Type elementType = collectionType.GetCollectionElementType()!;
            Type genericType = collectionGenericDefinition.MakeGenericType(elementType);

            if (genericType == collectionType)
            {
                return true;
            }

            if (collectionType.IsAssignableToClass(genericType))
            {
                return true;
            }

            return genericType.GetInterfaces().Any(i => i == collectionType || i.IsAssignableFrom(collectionType));
        }
    }
}
