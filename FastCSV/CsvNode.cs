using System;
using System.Collections.Generic;
using System.Reflection;
using FastCSV.Converters;

namespace FastCSV
{
    /// <summary>
    /// Holds information about a csv property.
    /// </summary>
    public record CsvNode(CsvPropertyInfo Info, string Name, object? Value) : ICsvPropertyInfo
    {
        /// <summary>
        /// The parent node.
        /// </summary>
        public CsvNode? Parent { get; set; }

        /// <summary>
        /// Children of this csv property data.
        /// </summary>
        public IReadOnlyList<CsvNode> Children { get; set; } = new List<CsvNode>();

        /// <inheritdoc/>
        public string OriginalName => Info.OriginalName;

        /// <inheritdoc/>
        public Type Type => Info.Type;

        /// <inheritdoc/>
        public MemberInfo Member => Info.Member;

        /// <inheritdoc/>
        public bool Ignore => Info.Ignore;

        /// <inheritdoc/>
        public ICsvValueConverter? Converter => Info.Converter;

        /// <inheritdoc/>
        public bool IsProperty => Info.IsProperty;

        /// <inheritdoc/>
        public bool IsField => Info.IsField;

        /// <inheritdoc/>
        public bool IsReadOnly => Info.IsReadOnly;

        /// <inheritdoc/>
        public object? GetValue(object target) => Info.GetValue(target);

        /// <inheritdoc/>
        public void SetValue(object target, object? value) => Info.SetValue(target, value);
    }
}