namespace FastCSV
{

    public static partial class CsvConverter
    {
        internal readonly struct CsvPropertyToSerialize
        {
            public CsvProperty Property { get; }

            public string ColumnName { get; }

            public object? Value { get; }

            public CsvPropertyToSerialize(CsvProperty property, string columnName, object? value)
            {
                Property = property;
                ColumnName = columnName;
                Value = value;
            }
        }
    }
}
