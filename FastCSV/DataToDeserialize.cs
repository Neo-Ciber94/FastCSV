namespace FastCSV
{
    internal readonly struct DataToDeserialize
    {
        public CsvProperty Property { get; }

        public object? Value { get; }

        public DataToDeserialize(CsvProperty property, object? value)
        {
            Property = property;
            Value = value;
        }
    }
}
