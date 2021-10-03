using FastCSV.Internal;
using System;
using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal class IListOfTConverter<T> : CollectionOfTConverter<IList<T>, T>
    {
        public override void AddItem(IList<T> collection, int index, Type elementType, T item)
        {
            if (elementType != typeof(T))
            {
                throw ThrowHelper.InvalidType(elementType, typeof(T));
            }

            collection.Add(item);
        }

        public override IList<T> CreateCollection(Type elementType, int length)
        {
            if (elementType != typeof(T))
            {
                throw ThrowHelper.InvalidType(elementType, typeof(T));
            }

            return new List<T>(length);
        }
    }
}
