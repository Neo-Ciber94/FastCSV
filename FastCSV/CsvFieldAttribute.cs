using System;

namespace FastCSV
{
    /// <summary>
    /// Provides an alias for a csv field.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CsvFieldAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFieldAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CsvFieldAttribute(string name) 
        { 
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("field name cannot be empty", nameof(name));
            }

            Name = name; 
        }

        /// <summary>
        /// Gets the name of the csv field.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }
    }
}
