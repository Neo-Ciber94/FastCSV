using System;
using System.Collections.Generic;
using System.Reflection;
using FastCSV.Converters;
using FastCSV.Utils;

// CsvMemberInfo
namespace FastCSV
{
    internal record CsvField
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
        /// The converter for this field.
        /// </summary>
        public IValueConverter? Converter { get;}

        /// <summary>
        /// Children fields of this field.
        /// </summary>
        public IList<CsvField> Children { get; set; } = new List<CsvField>();

        public bool IsProperty => Member is PropertyInfo;

        public bool IsField => Member is FieldInfo;

        public CsvField(string originalName, string name, object? value, Type type, MemberInfo member, bool ignore, IValueConverter? valueConverter)
        {
            OriginalName = originalName;
            Name = name;
            Value = value;
            Type = type;
            Member = member;
            Ignore = ignore;
            Converter = valueConverter;
        }

        public CsvField(string name, object? value, Type type, MemberInfo member, bool ignore, IValueConverter? valueConverter) 
            : this(name, name, value, type, member, ignore, valueConverter) { }
    }
}
