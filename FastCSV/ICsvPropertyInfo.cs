using System;
using System.Reflection;
using FastCSV.Converters;

namespace FastCSV
{
    /// <summary>
    /// Provides information about how serialize/deserialize a csv type property.
    /// </summary>
    public interface ICsvPropertyInfo
    {
        /// <summary>
        /// Original name of the field.
        /// </summary>
        string OriginalName { get; }

        /// <summary>
        /// Type of the field.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Source of the value with is either a field or an property.
        /// </summary>
        MemberInfo Member { get; }

        /// <summary>
        /// Whether the field should be ignored or not.
        /// </summary>
        bool Ignore { get; }

        /// <summary>
        /// The converter for this property.
        /// </summary>
        ICsvValueConverter? Converter { get; }

        /// <summary>
        /// Whether this instance is a property.
        /// </summary>
        public bool IsProperty { get; }

        /// <summary>
        /// Whether this instance is a field.
        /// </summary>
        public bool IsField { get; }

        /// <summary>
        /// Whether this instance is read-only.
        /// </summary>
        public bool IsReadOnly { get; }

        /// <summary>
        /// Sets a value to the member of the target.
        /// </summary>
        /// <param name="target">The instance to set the value.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(object target, object? value);

        /// <summary>
        /// Gets the value of the member of the target.
        /// </summary>
        /// <param name="target">The instance to get the value.</param>
        /// <returns>Get value of the target.</returns>
        public object? GetValue(object target);
    }
}