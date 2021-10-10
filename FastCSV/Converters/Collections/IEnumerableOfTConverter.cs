using FastCSV.Internal;
using FastCSV.Utils;
using System;
using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal abstract class IEnumerableOfTConverter<TCollection, TElement> : CsvCollectionConverter<TCollection, TElement> where TCollection : IEnumerable<TElement>
    {
        public abstract TCollection CreateCollection(int length);

        public override bool CanConvert(Type type)
        {
            return type.IsEnumerableType() && typeof(TCollection).IsAssignableTo(type);
        }

        public abstract void AddItem(ref TCollection collection, int index, TElement item);

        public override void AddItem(ref TCollection collection, int index, Type elementType, TElement item)
        {
            if (elementType != typeof(TElement))
            {
                throw ThrowHelper.InvalidType(elementType, typeof(TElement));
            }

            AddItem(ref collection, index, item);
        }

        public override TCollection CreateCollection(Type elementType, int length)
        {
            if (elementType != typeof(TElement))
            {
                throw ThrowHelper.InvalidType(elementType, typeof(TElement));
            }

            return CreateCollection(length);
        }

        public override bool TrySerialize(TCollection value, ref CsvSerializeState state)
        {
            Type elementType = typeof(TElement);

            foreach (TElement obj in value)
            {
                string? s = obj?.ToString();

                if (s == null)
                {
                    state.WriteNull();
                }
                else
                {
                    ICsvValueConverter? converter = GetElementConverter(state.Options, elementType, state.Converter);

                    if (converter == null || !converter.TrySerialize(obj, elementType, ref state))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
