using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV.Converters.Internal
{
    public abstract partial class CsvCollectionDeserializer // CsvCollectionDeserializer
    {
        private static CsvCollectionDeserializer? s_arrayConverter;
        public static CsvCollectionDeserializer ArrayConverter
        {
            get
            {
                if (s_arrayConverter == null)
                {
                    s_arrayConverter = new CsvIListDeserializer<Array>((elementType, length) =>
                    {
                        return Array.CreateInstance(elementType, length)!;
                    });
                }

                return s_arrayConverter;
            }
        }

        private static CsvCollectionDeserializer? s_listConverter;
        public static CsvCollectionDeserializer ListConverter
        {
            get
            {
                if (s_listConverter == null)
                {
                    static IList ListFactory(Type elementType, int length)
                    {
                        Type genericList = typeof(List<>).MakeGenericType(elementType);
                        return (IList)Activator.CreateInstance(genericList, length)!;
                    }

                    s_listConverter = new CsvIListDeserializer<IList>(ListFactory);
                }

                return s_listConverter;
            }
        }

        public static CsvCollectionDeserializer? GetConverterForType(Type type)
        {
            if (type.IsArray)
            {
                
                return ArrayConverter;
            }

            switch (type)
            {
                case Type _ when typeof(IEnumerable).IsAssignableFrom(type):
                case Type _ when typeof(ICollection).IsAssignableFrom(type):
                case Type _ when typeof(IList).IsAssignableFrom(type):
                case Type _ when typeof(IReadOnlyList<>) == type.GetGenericTypeDefinition():
                case Type _ when typeof(IReadOnlyCollection<>) == type.GetGenericTypeDefinition():
                case Type _ when typeof(ICollection<>) == type.GetGenericTypeDefinition():
                case Type _ when typeof(IList<>) == type.GetGenericTypeDefinition():
                    return ListConverter;
            }

            return null;
        }
    }
}