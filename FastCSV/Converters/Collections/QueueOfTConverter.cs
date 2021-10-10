using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal class QueueOfTConverter<T> : IEnumerableOfTConverter<Queue<T>, T>
    {
        public override void AddItem(ref Queue<T> collection, int index, T item)
        {
            collection.Enqueue(item);
        }

        public override Queue<T> CreateCollection(int length)
        {
            return new Queue<T>(length);
        }
    }
}
