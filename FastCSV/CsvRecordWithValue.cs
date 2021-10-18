using System.Runtime.CompilerServices;

namespace FastCSV
{
    internal struct CsvRecordWithValue<T>
    {
        internal readonly T _value;
        private readonly CsvRecord _record;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecordWithValue(T value, CsvFormat format)
        {
            _record = CsvRecord.From(value, format);
            _value = value;
        }
        
        public CsvRecord Record 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _record;
        }

        public T Value 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out CsvRecord record, out T value)
        {
            record = Record;
            value = _value;
        }

        public override string ToString()
        {
            return _value?.ToString() ?? string.Empty;
        }
    }
}
