using System;
using System.Reflection;
using FastCSV.Converters;
using FastCSV.Extensions;

namespace FastCSV
{
    /// <summary>
    /// Represents a field or property related to a object.
    /// </summary>
    public record CsvPropertyInfo : ICsvPropertyInfo
    {
        /// <inheritdoc/>
        public string OriginalName { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public MemberInfo Member { get; }

        /// <inheritdoc/>
        public bool Ignore { get; }

        /// <inheritdoc/>
        public ICsvValueConverter? Converter { get; }

        /// <inheritdoc/>
        public bool IsProperty => Member is PropertyInfo;

        /// <inheritdoc/>
        public bool IsField => Member is FieldInfo;

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get
            {
                return Member switch
                {
                    PropertyInfo p => p.CanRead && !p.CanWrite,
                    FieldInfo f => f.IsInitOnly,
                    _ => throw new Exception("Unreachable")
                };
            }
        }

        /// <summary>
        /// Constructs a new <see cref="CsvPropertyInfo"/>.
        /// </summary>
        /// <param name="originalName">The name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="member">The target property/field.</param>
        /// <param name="ignore">Whether if ignore this property.</param>
        /// <param name="valueConverter">The converter for this property type.</param>
        public CsvPropertyInfo(string originalName, Type type, MemberInfo member, bool ignore, ICsvCustomConverter? valueConverter)
        {
            OriginalName = originalName;
            Name = name;
            Value = value;
            Type = type;
            Member = member;
            Ignore = ignore;
            Converter = valueConverter;
        }

        /// <inheritdoc/>
        public void SetValue(object target, object? value)
        {
            Member.SetValue(target, value);
        }

        /// <inheritdoc/>
        public object? GetValue(object target)
        {
            return Member.GetValue(target);
        }
    }
}
