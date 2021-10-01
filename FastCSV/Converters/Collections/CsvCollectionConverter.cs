using System;
using System.Diagnostics;

namespace FastCSV.Converters.Collections
{
    /// <summary>
    /// Provides a mechanism for serialize a collection of values.
    /// </summary>
    /// <typeparam name="TCollection">Type of the collection to produces.</typeparam>
    /// <typeparam name="TElement">The type of the collection elements.</typeparam>
    public abstract class CsvCollectionConverter<TCollection, TElement> : ICsvValueConverter<TCollection>
    {
        /// <summary>
        /// Creates the collection.
        /// </summary>
        /// <param name="elementType">Type of the collection elements.</param>
        /// <param name="length">The min capacity of the collection.</param>
        /// <returns>Returns a collection of type <see cref="TCollection"/>.</returns>
        public abstract TCollection CreateCollection(Type elementType, int length);

        /// <summary>
        /// Adds a item to the specified collection.
        /// </summary>
        /// <param name="collection">The collection to insert the items.</param>
        /// <param name="index">The index when insert the value.</param>
        /// <param name="elementType">The type of the element to insert.</param>
        /// <param name="item">The element to insert.</param>
        public abstract void AddItem(TCollection collection, int index, Type elementType, TElement item);

        /// <summary>
        /// Gets a <see cref="ICsvValueConverter"/> for the given type.
        /// </summary>
        /// <param name="options">The options used.</param>
        /// <param name="property">The current property.</param>
        /// <returns></returns>
        public virtual ICsvValueConverter GetElementConverter(CsvConverterOptions options, Type elementType, ICsvValueConverter? converter = null)
        {
            if (converter != null && converter.CanConvert(elementType))
            {
                return converter;
            }

            converter = CsvConverter.GetConverter(elementType, options, converter);

            if (converter == null)
            {
                throw new InvalidOperationException($"No converter found for type {elementType}");
            }

            return converter;
        }

        public abstract bool TrySerialize(TCollection value, ref CsvSerializeState state);

        public virtual bool TryDeserialize(out TCollection value, ref CsvDeserializeState state)
        {
            value = default!;
            CollectionHandling? collectionHandling = state.Options.CollectionHandling;

            if (collectionHandling == null)
            {
                throw new ArgumentException("CollectionHandling was not provided");
            }

            if (state.Record == null)
            {
                throw new ArgumentException("Collection converter requires to read a CsvRecord");
            }

            CsvRecord record = state.Record;
            CsvHeader? header = record.Header;

            if (header == null)
            {
                throw new InvalidOperationException("Header cannot be null when deserializing collections");
            }

            int length = GetCollectionLength(record, state.ColumnIndex, state.Options);
            TCollection collection = CreateCollection(state.ElementType, length);
            CsvPropertyInfo? property = state.Property;
            int index = state.ColumnIndex;
            int endIndex = index + length;
            int count = 0;

            while (index < endIndex)
            {
                string itemName = collectionHandling.ItemName;
                Type typeToConvert = state.ElementType;

                if (!header[index].StartsWith(itemName))
                {
                    break;
                }

                ReadOnlySpan<char> indexString = header[index].AsSpan(itemName.Length);

                if (!int.TryParse(indexString, out int itemIndex))
                {
                    break;
                }

                if (itemIndex != ++count)
                {
                    throw new ArgumentOutOfRangeException($"Expected {itemName}{index + 1} but was {itemName}{itemIndex}");
                }

                ICsvValueConverter? converter = GetElementConverter(state.Options, state.ElementType, property?.Converter);

                if (!converter.TryDeserialize(out object? element, typeToConvert, ref state))
                {
                    return false;
                }

                AddItem(collection, count - 1, typeToConvert, (TElement)element!);
                index += 1;
            }

            value = collection;
            return true;
        }

        public virtual bool CanConvert(Type type)
        {
            return typeof(TCollection) == type;
        }

        /// <summary>
        /// Calculates the expected length for the collection.
        /// </summary>
        /// <param name="record">The source record.</param>
        /// <param name="startIndex">The start index of the elements.</param>
        /// <param name="options">The csv options.</param>
        /// <returns>The expected number of elements in the collection.</returns>
        protected int GetCollectionLength(CsvRecord record, int startIndex, CsvConverterOptions options)
        {
            var collectionHandling = options.CollectionHandling;
            var header = record.Header;
            var count = 0;

            if (header == null)
            {
                throw new InvalidOperationException("Header cannot be null");
            }

            if (collectionHandling == null)
            {
                throw new InvalidOperationException("CollectionHandling cannot be null");
            }

            var itemName = collectionHandling.ItemName;
            int index = startIndex;

            while (index < record.Length)
            {
                if (!header[index].StartsWith(itemName))
                {
                    break;
                }

                var indexString = header[index].AsSpan(itemName.Length);

                if (!int.TryParse(indexString, out int itemIndex))
                {
                    break;
                }

                if (itemIndex != ++count)
                {
                    throw new ArgumentOutOfRangeException($"Expected {itemName}{index + 1} but was {itemName}{itemIndex}");
                }

                index += 1;
            }

            return count;
        }
    }
}
