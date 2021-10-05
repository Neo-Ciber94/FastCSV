using FastCSV.Internal;
using System;
using System.Collections.Generic;

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

            if (property == null)
            {
                throw new InvalidOperationException("Cannot find a CsvPropertyInfo to deserialize the collection");
            }

            CsvConverterOptions options = state.Options;

            for (int i = 0; i < state.Count; i++)
            {
                Type elementType = state.ElementType;
                string stringValue = state.Read(i);

                if (elementType == typeof(object))
                {
                    // Fallback to string if not type was found
                    elementType = GuessTypeOrNull(stringValue, options.TypeGuessers) ?? typeof(string);
                }

                CsvDeserializeState elementState = new CsvDeserializeState(options, elementType, stringValue);
                ICsvValueConverter converter = GetElementConverter(options, elementType, property.Converter);

                if (!converter.TryDeserialize(out object? result, elementType, ref elementState))
                {
                    return false;
                }

                AddItem(collection, i, elementType, (TElement)result!);
            }

            value = collection;
            return true;
        }

        private static Type? GuessTypeOrNull(string s, IReadOnlyList<ITypeGuesser> typeGuesses)
        {
            if (typeGuesses.Count > 0)
            {
                foreach(var guesser in typeGuesses)
                {
                    Type? type = guesser.GetTypeFromString(s);

                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            return TypeHelper.GetTypeFromString(s);
        }
    }
}
