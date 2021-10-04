namespace FastCSV
{

    public static partial class CsvConverter
    {
        internal readonly struct CsvPropertyToDeserialize
        {
            public CsvPropertyInfo Property { get; }

            public object? Value { get; }

            public CsvPropertyToDeserialize(CsvPropertyInfo property, object? value)
            {
                Property = property;
                Value = value;
            }
        }
    }
}
