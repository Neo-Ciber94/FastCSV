using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal class ListOfTConverter<T> : CollectionOfTConverter<List<T>, T>
    {
        public override List<T> CreateCollection(int length)
        {
            return new List<T>(length);
        }
    }
}
