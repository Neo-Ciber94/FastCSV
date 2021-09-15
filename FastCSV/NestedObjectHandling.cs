namespace FastCSV
{
    /// <summary>
    /// Defines how to handle nested objects when serializing/deserializing.
    /// </summary>
    public sealed record NestedObjectHandling
    {
        /// <summary>
        /// Default nested object handling with a max depth of 8.
        /// </summary>
        public static NestedObjectHandling Default { get; } = new NestedObjectHandling { MaxDepth = 8 };

        /// <summary>
        /// Max depth allowed for nested objects.
        /// </summary>
        public int MaxDepth { get; init; }

        /// <summary>
        /// How handle nested objects reference loops, by default throws an error.
        /// </summary>
        public ReferenceLoopHandling ReferenceLoopHandling { get; init; } = ReferenceLoopHandling.Error;
    }
}