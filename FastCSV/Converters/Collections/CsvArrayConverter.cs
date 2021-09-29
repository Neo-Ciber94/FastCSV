using System;

namespace FastCSV.Converters.Collections
{
    public class CsvArrayConverter : CsvCollectionConverter<Array, object?>
    {
        public override void AddItem(Array collection, int index, Type elementType, object? item)
        {
            collection.SetValue(item, index);
        }

        public override bool CanConvert(Type type)
        {
            return type.IsArray;
        }

        public override Array CreateCollection(Type elementType, int length)
        {
            return Array.CreateInstance(elementType, length);
        }

        public override bool TrySerialize(Array value, ref CsvSerializeState state)
        {
            foreach (var obj in value)
            {
                if (obj == null)
                {
                    state.WriteNull();
                }
                else
                {
                    var elementType = obj.GetType();
                    var converter = GetConverter(state.Options, elementType, state.Converter);
                    if (!converter.TrySerialize(obj, elementType, ref state))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
