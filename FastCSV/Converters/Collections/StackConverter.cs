using System;
using System.Collections;

namespace FastCSV.Converters.Collections
{
    internal class StackConverter : CollectionConverter<Stack>
    {
        public override void AddItem(Stack collection, int index, Type elementType, object? item)
        {
            collection.Push(item);
        }

        public override Stack CreateCollection(Type elementType, int length)
        {
            return new Stack(length);
        }
    }
}
