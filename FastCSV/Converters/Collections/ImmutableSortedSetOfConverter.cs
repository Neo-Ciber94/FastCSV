using System.Collections.Immutable;

namespace FastCSV.Converters.Collections
{
    internal class ImmutableSortedSetOfTConverter<T> : IEnumerableOfTConverter<ImmutableSortedSet<T>, T>
    {
        public override void AddItem(ref ImmutableSortedSet<T> collection, int index, T item)
        {
            collection = collection.Add(item);
        }

        public override ImmutableSortedSet<T> CreateCollection(int length)
        {
            return ImmutableSortedSet<T>.Empty;
        }
    }
}
