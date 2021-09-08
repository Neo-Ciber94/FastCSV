using System;
using FastCSV.Converters;

namespace FastCSV
{
    /// <summary>
    /// Provides a <see cref="IValueConverter"/> for the decorated field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class CsvValueConverterAttribute : Attribute
    {
        /// <summary>
        /// Constructs a new <see cref="CsvValueConverterAttribute"/>.
        /// </summary>
        /// <param name="converterType">Type of the converter.</param>
        public CsvValueConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }

        /// <summary>
        /// Gets the type of the converter.
        /// </summary>
        public Type ConverterType { get; }
    }
}
