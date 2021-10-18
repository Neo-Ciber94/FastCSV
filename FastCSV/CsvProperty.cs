using System;
using System.Collections.Generic;
using System.Reflection;
using FastCSV.Converters;
using FastCSV.Extensions;

namespace FastCSV
{
    /// <summary>
    /// Represents a field or property related to a object.
    /// </summary>
    public record CsvProperty
    {
        /// <summary>
        /// Original name of the field.
        /// </summary>
        public string OriginalName { get; }

        /// <summary>
        /// Name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value of the field.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Type of the field.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Source of the value with is either a field or an property.
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Whether the field should be ignored or not.
        /// </summary>
        public bool Ignore { get; }

        /// <summary>
        /// The converter for this property.
        /// </summary>
        public ICsvValueConverter? Converter { get;}

        /// <summary>
        /// Children fields of this field.
        /// </summary>
        public IReadOnlyList<CsvProperty> Children { get; set; } = new List<CsvProperty>();

        /// <summary>
        /// Whether this instance is a property.
        /// </summary>
        public bool IsProperty => Member is PropertyInfo;

        /// <summary>
        /// Whether this instance is a field.
        /// </summary>
        public bool IsField => Member is FieldInfo;

        /// <summary>
        /// Whether this instance is read-only.
        /// </summary>
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

        public CsvProperty(string originalName, string name, object? value, Type type, MemberInfo member, bool ignore, ICsvCustomConverter? valueConverter)
        {
            OriginalName = originalName;
            Name = name;
            Value = value;
            Type = type;
            Member = member;
            Ignore = ignore;
            Converter = valueConverter;
        }

        public CsvProperty(string name, object? value, Type type, MemberInfo member, bool ignore, ICsvCustomConverter? valueConverter) 
            : this(name, name, value, type, member, ignore, valueConverter) { }

        /// <summary>
        /// Sets a value to the member of the target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public void SetValue(object target, object? value)
        {
            Member.SetValue(target, value);
        }

        /// <summary>
        /// Gets the value of the member of the target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public object? GetValue(object target)
        {
            return Member.GetValue(target);
        }
    }
}
