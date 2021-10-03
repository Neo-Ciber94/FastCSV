using FastCSV.Internal;
using System;
using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal class ListOfTConverter<T> : CollectionOfTConverter<List<T>, T>
    {
        public override bool CanConvert(Type type)
        {
            /*
             * ListOfTConverter<T> can be used to convert any type implemented by List<T>:
             * - List<T>
             * - IList<T>
             * - IReadOnlyList<T>
             * - ICollection<T>
             * - IReadOnlyCollection<T>
             * - IEnumerable<T>
             */
            return typeof(List<T>).IsAssignableTo(type);
        }

        public override void AddItem(List<T> collection, int index, Type elementType, T item)
        {
            if (elementType != typeof(T))
            {
                throw ThrowHelper.InvalidType(elementType, typeof(T));
            }

            collection.Add(item);
        }

        public override List<T> CreateCollection(Type elementType, int length)
        {
            if (elementType != typeof(T))
            {
                throw ThrowHelper.InvalidType(elementType, typeof(T));
            }

            return new List<T>(length);
        }
    }
}
