using System;
using System.Collections.Generic;
using System.Reflection;
using FastCSV.Converters;
using FastCSV.Extensions;

namespace FastCSV
{
    public record CsvPropertyData(CsvPropertyInfo Info, string Name, object? Value)
    {
        public IReadOnlyList<CsvPropertyData> Children { get; set; } = new List<CsvPropertyData>();
    }

    /// <summary>
    /// Represents a field or property related to a object.
    /// </summary>
    public record CsvPropertyInfo
    {
        /// <summary>
        /// Original name of the field.
        /// </summary>
        public string OriginalName { get; }

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

        public CsvPropertyInfo(string originalName, Type type, MemberInfo member, bool ignore, ICsvCustomConverter? valueConverter)
        {
            OriginalName = originalName;
            Type = type;
            Member = member;
            Ignore = ignore;
            Converter = valueConverter;
        }

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