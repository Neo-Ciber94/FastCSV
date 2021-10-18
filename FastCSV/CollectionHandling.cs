namespace FastCSV
{
    /// <summary>
    /// Options for handling a collection of items in a csv.
    /// </summary>
    public record CollectionHandling
    {
        /// <summary>
        /// Default collection handling.
        /// </summary>
        public static CollectionHandling Default { get; } = new CollectionHandling();

        private readonly string _tag = "item";

        /// <summary>
        /// Suffix used for serialized and deserialized items.
        /// 
        /// <para>
        /// This is used to locate the items of an collection.
        /// </para>
        /// </summary>
        public string Tag
        {
            get => _tag;
            init
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new System.Exception($"{nameof(Tag)} cannot be empty");
                }

                _tag = value;
            }
        }
    }
}