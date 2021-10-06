using FastCSV.Utils;
using System;
using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal abstract class CollectionOfTConverter<TCollection, TElement> : CsvCollectionConverter<TCollection, TElement> where TCollection : ICollection<TElement>
    {
        public override bool CanConvert(Type type)
        {
            return type.IsEnumerableType() && typeof(TCollection).IsAssignableTo(type);
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
