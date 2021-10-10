using System.Collections.Concurrent;

namespace FastCSV.Converters.Collections
{
    internal class BlockingCollectionOfTConverter<T> : IEnumerableOfTConverter<BlockingCollection<T>, T>
    {
        public override void AddItem(ref BlockingCollection<T> collection, int index, T item)
        {
            collection.Add(item);
        }

        public override BlockingCollection<T> CreateCollection(int length)
        {
            return new BlockingCollection<T>(length);
        }
    }
}
