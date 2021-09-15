namespace FastCSV
{
    /// <summary>
    /// How handle reference loops when serializing and deserializing.
    /// </summary>
    public enum ReferenceLoopHandling
    {
        /// <summary>
        /// Throws and error when a reference loop is found.
        /// </summary>
        Error, 

        /// <summary>
        /// Ignore reference loop.
        /// </summary>
        Ignore, 

        /// <summary>
        /// Attemps to serialize reference loop until a max depth.
        /// </summary>
        Serialize
    }
}