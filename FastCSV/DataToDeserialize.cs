namespace FastCSV
{
    internal readonly struct DataToDeserialize
    {
        public CsvPropertyInfo Property { get; }

        public object? Value { get; }

        public DataToDeserialize(CsvPropertyInfo property, object? value)
        {
            Property = property;
            Value = value;
        }
    }
}
