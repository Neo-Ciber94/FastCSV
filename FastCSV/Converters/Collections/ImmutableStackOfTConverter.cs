using System.Collections.Immutable;

namespace FastCSV.Converters.Collections
{
    internal class ImmutableStackOfTConverter<T> : IEnumerableOfTConverter<ImmutableStack<T>, T>
    {
        public override void AddItem(ref ImmutableStack<T> collection, int index, T item)
        {
            collection = collection.Push(item);
        }

        public override ImmutableStack<T> CreateCollection(int length)
        {
            return ImmutableStack<T>.Empty;
        }
    }
}
