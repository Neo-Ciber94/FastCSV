using System.Collections.Immutable;

namespace FastCSV.Converters.Collections
{
    internal class ImmutableQueueOfTConverter<T> : IEnumerableOfTConverter<ImmutableQueue<T>, T>
    {
        public override void AddItem(ref ImmutableQueue<T> collection, int index, T item)
        {
            collection = collection.Enqueue(item);
        }

        public override ImmutableQueue<T> CreateCollection(int length)
        {
            return ImmutableQueue<T>.Empty;
        }
    }
}
