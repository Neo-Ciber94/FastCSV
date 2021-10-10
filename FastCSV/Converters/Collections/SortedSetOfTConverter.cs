using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal class SortedSetOfTConverter<T> : CollectionOfTConverter<SortedSet<T>, T>
    {
        public override SortedSet<T> CreateCollection(int length)
        {
            return new SortedSet<T>();
        }
    }
}
