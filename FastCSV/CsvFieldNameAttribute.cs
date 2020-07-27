using System;

namespace FastCSV
{
    /// <summary>
    /// Provides an alias for a csv field.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CsvFieldNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFieldNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CsvFieldNameAttribute(string name) { Name = name; }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }
    }
}
