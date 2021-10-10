using System.Collections.Concurrent;

namespace FastCSV.Converters.Collections
{
    internal class ConcurrentStackOfTConverter<T> : IEnumerableOfTConverter<ConcurrentStack<T>, T>
    {
        public override void AddItem(ref ConcurrentStack<T> collection, int index, T item)
        {
            collection.Push(item);
        }

        public override ConcurrentStack<T> CreateCollection(int length)
        {
            return new ConcurrentStack<T>();
        }
    }
}
