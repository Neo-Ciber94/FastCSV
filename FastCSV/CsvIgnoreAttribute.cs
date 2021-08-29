using System;

namespace FastCSV
{
    /// <summary>
    /// Marks a field or property to be ignored by a csv converter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class CsvIgnoreAttribute : Attribute {}
}
