using System;

namespace FastCSV.Converters.Internal
{
    public abstract class CsvCollectionConverter
    {
        protected abstract object CreateCollection(Type? elementType, int length);

        protected abstract void AddItem(object collection, int index, object? item);
    }

    public abstract class CsvCollectionConverter<TCollection, TElement> : CsvCollectionConverter
    {
        protected abstract TCollection CreateCollection(int length);

        protected abstract void AddItem(TCollection collection, int index, TElement? item);

        protected override object CreateCollection(Type? elementType, int length)
        {
            return CreateCollection(length)!;
        }

        protected override void AddItem(object collection, int index, object? item)
        {
            AddItem((TCollection)collection, index, (TElement?)item);
        }
    }

    public class CsvArrayConverter : CsvCollectionConverter
    {
        private Type? _elementType;

        protected override void AddItem(object collection, int index, object? item)
        {
            throw new NotImplementedException();
        }

        protected override object CreateCollection(Type? elementType, int length)
        {
            _elementType = elementType ?? typeof(object);
            return Array.CreateInstance(_elementType, length);
        }
    }
}