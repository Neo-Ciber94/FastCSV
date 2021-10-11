using System;
using System.Collections.Generic;
using FastCSV.Extensions;

namespace FastCSV.Converters.Collections
{
    internal abstract class CollectionOfTConverter<TCollection, TElement> : IEnumerableOfTConverter<TCollection, TElement> where TCollection : ICollection<TElement>
    {
        public override void AddItem(ref TCollection collection, int index, TElement item)
        {
            collection.Add(item);
        }

        public override bool CanConvert(Type type)
        {
            return type.IsEnumerableType() && typeof(TCollection).IsAssignableTo(type);
        }
    }
}
