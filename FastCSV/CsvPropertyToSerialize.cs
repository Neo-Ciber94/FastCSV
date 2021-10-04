namespace FastCSV
{

    public static partial class CsvConverter
    {
        internal readonly struct CsvPropertyToSerialize
        {
            public CsvPropertyInfo Property { get; }

            public string ColumnName { get; }

            public object? Value { get; }

            public CsvPropertyToSerialize(CsvPropertyInfo property, string columnName, object? value)
            {
                Property = property;
                ColumnName = columnName;
                Value = value;
            }
        }
    }
}
