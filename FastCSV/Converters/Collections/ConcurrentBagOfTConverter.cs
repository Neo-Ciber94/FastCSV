using System.Collections.Concurrent;

namespace FastCSV.Converters.Collections
{
    internal class ConcurrentBagOfTConverter<T> : IEnumerableOfTConverter<ConcurrentBag<T>, T>
    {
        public override void AddItem(ref ConcurrentBag<T> collection, int index, T item)
        {
            collection.Add(item);
        }

        public override ConcurrentBag<T> CreateCollection(int length)
        {
            return new ConcurrentBag<T>();
        }
    }
}
