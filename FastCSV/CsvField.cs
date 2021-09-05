using System;
using System.Reflection;
using FastCSV.Utils;

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
        public Either<FieldInfo, PropertyInfo> Source { get; }

        /// <summary>
        /// Whether the field should be ignored or not.
        /// </summary>
        public bool Ignore { get; set; }

        public CsvField(string originalName, string name, object? value, Type type, Either<FieldInfo, PropertyInfo> source, bool ignore)
        {
            OriginalName = originalName;
            Name = name;
            Value = value;
            Type = type;
            Source = source;
            Ignore = ignore;
        }

        public CsvField(string name, object? value, Type type, Either<FieldInfo, PropertyInfo> source, bool ignore) 
            : this(name, name, value, type, source, ignore) { }
    }
}
