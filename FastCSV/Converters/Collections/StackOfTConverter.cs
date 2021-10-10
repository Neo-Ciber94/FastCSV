using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal class StackOfTConverter<T> : IEnumerableOfTConverter<Stack<T>, T>
    {
        public override void AddItem(ref Stack<T> collection, int index, T item)
        {
            collection.Push(item);
        }

        public override Stack<T> CreateCollection(int length)
        {
            return new Stack<T>(length);
        }
    }
}
