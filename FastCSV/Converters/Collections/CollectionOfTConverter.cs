using System;
using System.Collections.Generic;

namespace FastCSV.Converters.Collections
{
    internal abstract class CollectionOfTConverter<TCollection, TElement> : CsvCollectionConverter<TCollection, TElement> where TCollection : ICollection<TElement>
    {
        public override bool TrySerialize(TCollection value, ref CsvSerializeState state)
        {
            foreach (TElement obj in value)
            {
                string? s = obj?.ToString();

                if (s == null)
                {
                    state.WriteNull();
                }
                else
                {
                    state.Write(s);
                }
            }

            return true;
        }
    }
}
