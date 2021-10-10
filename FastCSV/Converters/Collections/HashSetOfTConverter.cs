using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal class HashSetOfTConverter<T> : CollectionOfTConverter<HashSet<T>, T>
    {
        public override HashSet<T> CreateCollection(int length)
        {
            return new HashSet<T>(length);
        }
    }
}
