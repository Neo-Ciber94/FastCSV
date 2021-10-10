using System.Collections.Immutable;

namespace FastCSV.Converters.Collections
{
    internal class ImmutableListOfTConverter<T> : IEnumerableOfTConverter<ImmutableList<T>, T>
    {
        public override void AddItem(ref ImmutableList<T> collection, int index, T item)
        {
            collection = collection.Add(item);
        }

        public override ImmutableList<T> CreateCollection(int length)
        {
            return ImmutableList<T>.Empty;
        }
    }
}
