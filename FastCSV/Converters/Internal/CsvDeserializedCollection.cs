namespace FastCSV.Converters.Internal
{
    /// <summary>
    /// Result of an <see cref="CsvCollectionDeserializer"/>.
    /// </summary>
    internal readonly struct CsvDeserializedCollection
    {
        /// <summary>
        /// The resulting collection.
        /// </summary>
        public object Collection { get; }

        /// <summary>
        /// The number of elements in the collection.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Constructs a new <see cref="CsvDeserializedCollection"/>.
        /// </summary>
        /// <param name="collection">The collection of items.</param>
        /// <param name="length">The length of the collection.</param>
        public CsvDeserializedCollection(object collection, int length)
        {
            Collection = collection;
            Length = length;
        }
    }
}
