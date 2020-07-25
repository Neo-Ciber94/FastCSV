namespace FastCSV
{
    /// <summary>
    /// Represents when add quotes to a record value
    /// </summary>
    public enum QuoteStyle
    {
        /// <summary>
        /// The always add quotes to the values.
        /// </summary>
        Always,
        /// <summary>
        /// The never add quotes to the values and if any, will be ignored.
        /// </summary>
        Never,
        /// <summary>
        /// Add quotes only when needed.
        /// </summary>
        WhenNeeded
    }
}
