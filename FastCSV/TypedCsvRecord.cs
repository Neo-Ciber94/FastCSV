using FastCSV.Utils;

namespace FastCSV
{
    internal struct TypedCsvRecord<T>
    {
        private Optional<CsvRecord> _record;

        public TypedCsvRecord(T value, CsvHeader header, CsvFormat format)
        {
            Value = value;
            _record = default;
            Format = format;
            Header = header;
        }

        public T Value { get; }

        public CsvFormat Format { get; }

        public CsvHeader Header { get; }

        public CsvRecord Record
        {
            get
            {
                if (!_record.HasValue)
                {
                    var values = CsvUtility.GetValues(Value);
                    var temp = new CsvRecord(Header, values, Format);
                    _record = new Optional<CsvRecord>(temp);
                }

                return _record.Value;
            }
        }
    }
}
