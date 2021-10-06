using FastCSV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
                Type genericDefinition = type.GetGenericTypeDefinition();

                switch (type)
                {
                    case Type _ when genericDefinition == typeof(List<>):
                    case Type _ when genericDefinition == typeof(IList<>):
                    case Type _ when genericDefinition == typeof(IReadOnlyList<>):
                    case Type _ when genericDefinition == typeof(ICollection<>):
                    case Type _ when genericDefinition == typeof(IReadOnlyCollection<>):
                    case Type _ when genericDefinition == typeof(IEnumerable<>):
                        {
                            var elementType = type.GetEnumerableElementType()!;
                            var listOfTConverter = GenericConverterFactory.CreateCollectionConverter(typeof(ListOfTConverter<>), elementType);
                            _converters.Add(type, listOfTConverter);
                            return listOfTConverter;
                        }
                    default:
                        break;
                }
            }

            // Fallback
            return GetNonGenericCollectionConverter(type);
        }

        private ICsvValueConverter? GetNonGenericCollectionConverter(Type type)
        {
            switch (type)
            {
                case Type _ when type == typeof(ArrayList):
                case Type _ when type == typeof(IList):
                case Type _ when type == typeof(ICollection):
                case Type _ when type == typeof(IEnumerable):
                    return GetOrCreateIListConverter();
                default:
                    break;
            }

            return null;
        }

        private static bool CanAssignToType(Type collectionGenericDefinition, Type collectionType)
        {
            Debug.Assert(collectionGenericDefinition.IsGenericTypeDefinition);
            Debug.Assert(collectionType.IsEnumerableType());

            Type elementType = collectionType.GetEnumerableElementType()!;
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
