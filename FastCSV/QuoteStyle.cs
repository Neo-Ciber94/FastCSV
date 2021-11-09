namespace FastCSV
{
    /// <summary>
    /// Represents when add quotes to a record value
    /// </summary>
    public enum QuoteStyle
    {
        /// <summary>
        /// Always add quotes to the values.
        /// </summary>
        Always,

        /// <summary>
        /// Never adds quotes to the values and if any will be ignored.
        /// </summary>
        Never,

        /// <summary>
        /// Add quotes only when needed.
        /// </summary>
        WhenNeeded,

        /// <summary>
        /// Keeps the quotes as they come from the source.
        /// </summary>
        Maintain,
    }
}
