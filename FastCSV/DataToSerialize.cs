namespace FastCSV
{
    internal readonly struct DataToSerialize
    {
        public CsvProperty Property { get; }

        public string ColumnName { get; }

        public object? Value { get; }

        public DataToSerialize(CsvProperty property, string columnName, object? value)
        {
            Property = property;
            ColumnName = columnName;
            Value = value;
        }
    }
}
