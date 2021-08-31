using System;
using System.Reflection;
using FastCSV.Utils;

namespace FastCSV
{
    internal record CsvField
    {
        public string OriginalName { get; }

        public string Name { get; }

        public object? Value { get; }

        public Type Type { get; }

        public Either<FieldInfo, PropertyInfo> Source { get; }

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
