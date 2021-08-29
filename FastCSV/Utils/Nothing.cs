
namespace FastCSV.Utils
{
    /// <summary>
    /// Represents the adsence of a value.
    /// </summary>
    public record Nothing
    {
        /// <summary>
        /// Gets the empty instance.
        /// </summary>
        public static Nothing Value { get; } = new Nothing();

        private Nothing() { }
    }
}
