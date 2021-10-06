using FastCSV.Utils;
using System;
using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal abstract class CollectionOfTConverter<TCollection, TElement> : IEnumerableOfTConverter<TCollection, TElement> where TCollection : ICollection<TElement>
    {
        public override void AddItem(TCollection collection, int index, TElement item)
        {
            collection.Add(item);
        }

        public override bool CanConvert(Type type)
        {
            return type.IsEnumerableType() && typeof(TCollection).IsAssignableTo(type);
        }
    }
}
