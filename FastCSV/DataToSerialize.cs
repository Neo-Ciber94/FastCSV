namespace FastCSV
{
    internal readonly struct DataToSerialize
    {
        public CsvNode Node { get; }

        public string ColumnName { get; }

        public object? Value { get; }

        public DataToSerialize(CsvNode node, string columnName, object? value)
        {
            Node = node;
            ColumnName = columnName;
            Value = value;
        }
    }
}
