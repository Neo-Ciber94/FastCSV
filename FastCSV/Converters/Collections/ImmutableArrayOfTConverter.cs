using System.Collections.Immutable;

namespace FastCSV.Converters.Collections
{
    internal class ImmutableArrayOfTConverter<T> : IEnumerableOfTConverter<ImmutableArray<T>, T>
    {
        public override void AddItem(ref ImmutableArray<T> collection, int index, T item)
        {
            collection = collection.Add(item);
        }

        public override ImmutableArray<T> CreateCollection(int length)
        {
            return ImmutableArray<T>.Empty;
        }
    }
}
