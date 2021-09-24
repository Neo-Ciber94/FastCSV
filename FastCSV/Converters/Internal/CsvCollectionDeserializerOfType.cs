using System;
using FastCSV.Utils;

namespace FastCSV.Converters.Internal
{
    /// <summary>
    /// Provides a mechanism for creating a collection from a csv.
    /// </summary>
    public abstract partial class CsvCollectionDeserializer
    {
        /// <summary>
        /// Converts a csv record into a collection.
        /// 
        /// <para>
        /// This method is only used internally.
        /// </para>
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="field">The field the collection belongs.</param>
        /// <param name="arrayHandling">The array handling options.</param>
        /// <param name="startIndex">The index on the record where the collection start.</param>
        /// <returns>The deserialed collection.</returns>
        internal CsvDeserializedCollection Convert(CsvRecord record, CsvField field, CollectionHandling arrayHandling, int startIndex)
        {
            Type elementType = field.Type.GetCollectionElementType()!;
            string itemName = arrayHandling.ItemName;
            int itemCount = 0;

            while (true)
            {
                int index = startIndex + itemCount;

                if (index >= record.Length)
                {
                    break;
                }

                string s = record.Header![index];

                if (s.StartsWith(itemName))
                {
                    ReadOnlySpan<char> numberString = s.AsSpan()[itemName.Length..];

                    if (!int.TryParse(numberString, out int count))
                    {
                        break;
                    }

                    if (count != (itemCount + 1))
                    {
                        throw new InvalidOperationException($"Invalid item order, expected {itemCount + 1} but was {count}");
                    }

                    itemCount += 1;
                }
                else
                {
                    break;
                }
            }

            object collection = CreateCollection(elementType, itemCount)!;

            for (int i = 0; i < itemCount; i++)
            {
                string csvValue = record[i + startIndex];
                object? value = CsvConverter.ParseString(csvValue, elementType);
                AddItem(collection, i, value);
            }

            return new CsvDeserializedCollection(collection, itemCount);
        }

        protected abstract object CreateCollection(Type elementType, int length);

        protected abstract void AddItem(object collection, int index, object? item);
    }

    /// <summary>
    /// Provides a mechanism for creating a collection from a csv.
    /// </summary>
    /// <typeparam name="TCollection">The of type the collection.</typeparam>
    /// <typeparam name="T">The type of the collection items</typeparam>
    public abstract class CsvCollectionDeserializerOfType<TCollection, T> : CsvCollectionDeserializer
    {
        protected abstract TCollection CreateCollectionOfType(Type elementType, int length);

        protected abstract void AddItemOfType(TCollection collection, int index, T? item);

        protected sealed override object CreateCollection(Type elementType, int length)
        {
            return CreateCollectionOfType(elementType, length)!;
        }

        protected sealed override void AddItem(object collection, int index, object? item)
        {
            AddItemOfType((TCollection)collection, index, (T?)item);
        }
    }
}
