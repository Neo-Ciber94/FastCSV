namespace FastCSV
{
    internal readonly struct DataToSerialize
    {
        public CsvPropertyData Property { get; }

        public string ColumnName { get; }

        public object? Value { get; }

        public DataToSerialize(CsvPropertyData property, string columnName, object? value)
        {
            Property = property;
            ColumnName = columnName;
            Value = value;
        }
    }
}
