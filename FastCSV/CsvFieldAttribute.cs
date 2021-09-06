using System;
using FastCSV.Converters;

namespace FastCSV
{
    /// <summary>
    /// Provides an alias for a csv field.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CsvFieldAttribute : Attribute
    {
        private readonly Type? _converterType;
        private readonly string? _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFieldAttribute"/> class.
        /// </summary>
        public CsvFieldAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFieldAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CsvFieldAttribute(string name) 
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the csv field.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string? Name
        {
            get => _name;

            init
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("field name cannot be empty");
                }

                _name = value;
            }
        }

        /// <summary>
        /// Gets the type of the <see cref="IValueConverter"/>.
        /// </summary>
        public Type? Converter 
        {
            get => _converterType;

            init
            {
                if (!typeof(IValueConverter).IsAssignableFrom(value))
                {
                    throw new ArgumentException($"Type {value} does not implements {typeof(IValueConverter)}");
                }

                _converterType = value;
            }
        }
    }
}