using System.Collections.Concurrent;

namespace FastCSV.Converters.Collections
{
    internal class ConcurrentQueueOfTConverter<T> : IEnumerableOfTConverter<ConcurrentQueue<T>, T>
    {
        public override void AddItem(ref ConcurrentQueue<T> collection, int index, T item)
        {
            collection.Enqueue(item);
        }

        public override ConcurrentQueue<T> CreateCollection(int length)
        {
            return new ConcurrentQueue<T>();
        }
    }
}
