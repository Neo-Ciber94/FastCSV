using System;
using System.Collections;

namespace FastCSV.Converters.Collections
{
    internal class ArrayListConverter : CollectionConverter<ArrayList>
    {
        public override bool CanConvert(Type type)
        {
            return typeof(ArrayList).IsAssignableTo(type);
        }

        public override void AddItem(ref ArrayList collection, int index, Type elementType, object? item)
        {
            collection.Add(item);
        }

        public override ArrayList CreateCollection(Type elementType, int length)
        {
            return new ArrayList(length);
        }
    }
}
