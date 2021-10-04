namespace FastCSV
{

    public static partial class CsvConverter
    {
        internal readonly struct CsvPropertyToDeserialize
        {
            public CsvProperty Property { get; }

            public object? Value { get; }

            public CsvPropertyToDeserialize(CsvProperty property, object? value)
            {
                Property = property;
                Value = value;
            }
        }
    }
}
