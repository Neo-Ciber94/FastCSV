using System;
using System.Collections;

namespace FastCSV.Converters.Collections
{
    internal class IListConverter : CollectionConverter<IList>
    {
        public override void AddItem(IList collection, int index, Type elementType, object? item)
        {
            collection.Add(item);
        }

        public override IList CreateCollection(Type elementType, int length)
        {
            return new ArrayList(length);
        }
    }
}
