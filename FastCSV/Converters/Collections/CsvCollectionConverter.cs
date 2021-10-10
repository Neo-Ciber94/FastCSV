using System;
using System.Runtime.CompilerServices;

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
        public abstract void AddItem(ref TCollection collection, int index, Type elementType, TElement item);

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

        /// <summary>
        /// Perform an operation over a collection before deserializing it.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>The same collection with any changes make into it, if any.</returns>
        protected virtual TCollection PrepareCollection(TCollection collection)
        {
            return collection;
        }

        /// <summary>
        /// Gets the type of the element at the given index.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        /// <param name="state">The current deserialization state.</param>
        /// <returns>The type of the element at the specified index.</returns>
        protected virtual Type GetElementTypeAt(int index, ref CsvDeserializeState state)
        {
            return state.ElementType;
        }

        public virtual bool CanConvert(Type type)
        {
            return typeof(TCollection) == type;
        }

        public abstract bool TrySerialize(TCollection value, ref CsvSerializeState state);

        public virtual bool TryDeserialize(out TCollection value, ref CsvDeserializeState state)
        {
            value = default!;

            TCollection collection = CreateCollection(state.ElementType, state.Count);
            CsvProperty? property = state.Property;
            CsvConverterOptions options = state.Options;

            for (int i = 0; i < state.Count; i++)
            {
                Type elementType = GetElementTypeAt(i, ref state);
                string stringValue = state.Read(i);

                CsvDeserializeState elementState = new CsvDeserializeState(options, elementType, stringValue);
                ICsvValueConverter converter = GetElementConverter(options, elementType, property?.Converter);

                if (!converter.TryDeserialize(out object? result, elementType, ref elementState))
                {
                    return false;
                }

                if (result is ITuple tuple)
                {
                    i += (tuple.Length - 1);
                }

                AddItem(ref collection, i, elementType, (TElement)result!);
            }

            value = PrepareCollection(collection);
            return true;
        }
    }
}
