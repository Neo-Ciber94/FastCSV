using System;
using System.Collections;

namespace FastCSV.Converters.Collections
{
    internal class QueueConverter : CollectionConverter<Queue>
    {
        public override void AddItem(ref Queue collection, int index, Type elementType, object? item)
        {
            collection.Enqueue(item);
        }

        public override Queue CreateCollection(Type elementType, int length)
        {
            return new Queue(length);
        }
    }
}
