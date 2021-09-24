namespace FastCSV
{
    /// <summary>
    /// Options used for serialize or deserialize a csv.
    /// </summary>
    public record CsvConverterOptions
    {
        /// <summary>
        /// A set of default <see cref="CsvConverterOptions"/> options.
        /// </summary>
        public static CsvConverterOptions Default { get; } = new CsvConverterOptions();

        /// <summary>
        /// Format used for the serialization or deserialization.
        /// </summary>
        public CsvFormat Format { get; init; } = CsvFormat.Default;

        /// <summary>
        /// If <c>true</c> class fields will be included during serialization/deserialization, by default only properties are included.
        /// Default is <c>false</c>.
        /// </summary>
        public bool IncludeFields { get; init; } = false;

        /// <summary>
        /// If <c>true</c> a header will be included during serialization/deserialization.
        /// Default is <c>true</c>.
        /// </summary>
        public bool IncludeHeader { get; init; } = true;

        /// <summary>
        /// Naming convention used.
        /// </summary>
        public CsvNamingConvention? NamingConvention { get; init; }

        /// <summary>
        /// Defines how handle nested objects.
        /// </summary>
        public NestedObjectHandling? NestedObjectHandling { get; init; }

        /// <summary>
        /// Defines how handle a collection of objects.
        /// </summary>
        public CollectionHandling? CollectionHandling { get; init; }

        /// <summary>
        /// The delimiter of the format.
        /// </summary>
        public char Delimiter => Format.Delimiter;

        /// <summary>
        /// The quote of the format.
        /// </summary>
        public char Quote => Format.Quote;

        /// <summary>
        /// The quote style of the format.
        /// </summary>
        public QuoteStyle Style => Format.Style;

        /// <summary>
        /// Whether ignore or not whitespaces when deserializing.
        /// </summary>
        public bool IgnoreWhitespace => Format.IgnoreWhitespace;
    }
}