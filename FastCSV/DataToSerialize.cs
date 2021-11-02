namespace FastCSV
{
    internal readonly struct DataToSerialize
    {
        public CsvNode Property { get; }

        public string ColumnName { get; }

        public object? Value { get; }

        public DataToSerialize(CsvNode property, string columnName, object? value)
        {
            Property = property;
            ColumnName = columnName;
            Value = value;
        }
    }
}
