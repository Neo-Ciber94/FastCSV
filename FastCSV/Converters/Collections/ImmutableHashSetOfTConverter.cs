using System.Collections.Immutable;

namespace FastCSV.Converters.Collections
{
    internal class ImmutableHashSetOfTConverter<T> : IEnumerableOfTConverter<ImmutableHashSet<T>, T>
    {
        public override void AddItem(ref ImmutableHashSet<T> collection, int index, T item)
        {
            collection = collection.Add(item);
        }

        public override ImmutableHashSet<T> CreateCollection(int length)
        {
            return ImmutableHashSet<T>.Empty;
        }
    }
}
