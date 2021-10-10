using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal class LinkedListOfTConverter<T> : CollectionOfTConverter<LinkedList<T>, T>
    {
        public override LinkedList<T> CreateCollection(int length)
        {
            return new LinkedList<T>();
        }
    }
}
