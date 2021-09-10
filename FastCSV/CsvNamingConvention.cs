namespace FastCSV
{
    /// <summary>
    /// Provides a name converter of the properties and variables to a csv field name.
    /// </summary>
    public abstract class CsvNamingConvention
    {
        /// <summary>
        /// A snake-case naming convention.
        /// </summary>
        public static CsvNamingConvention SnakeCase { get; } = new SnakeCaseNamingConvention();

        /// <summary>
        /// Converts the given property or variable name.
        /// </summary>
        /// <param name="name">The name to convert.</param>
        /// <returns>The converted name.</returns>
        public abstract string Convert(string name);
    }
}
