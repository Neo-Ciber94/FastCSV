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

        private readonly string _itemName = "item";

        /// <summary>
        /// Suffix used for serialized and deserialized items.
        /// </summary>
        public string ItemName
        {
            get => _itemName;
            init
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new System.Exception($"{nameof(ItemName)} cannot be empty");
                }

                _itemName = value;
            }
        }
    }
}