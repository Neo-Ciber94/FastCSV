using System;
using System.Collections;

namespace FastCSV.Converters.Internal
{
    internal class CsvIListDeserializer<TList> : CsvCollectionDeserializerOfType<TList, object> where TList: IList
    {
        private readonly Func<Type, int, TList> factory;

        public CsvIListDeserializer(Func<Type, int, TList> factory)
        {
            this.factory = factory;
        }

        protected override void AddItemOfType(TList collection, int index, object? item)
        {
            if (collection is Array array)
            {
                array.SetValue(item, index);
            }
            else
            {
                collection.Add(item);
            }
        }

        protected override TList CreateCollectionOfType(Type elementType, int length)
        {
            return factory(elementType, length);
        }
    }
}
