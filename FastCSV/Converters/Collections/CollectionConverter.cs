using System.Collections;

namespace FastCSV.Converters.Collections
{
    internal abstract class CollectionConverter<TCollection> : CsvCollectionConverter<TCollection, object?> where TCollection: ICollection
    {
        public override bool TrySerialize(TCollection value, ref CsvSerializeState state)
        {
            foreach (object? obj in value)
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
