using System.Runtime.CompilerServices;

namespace FastCSV
{
    internal struct CsvRecordWithValue<T>
    {
        internal readonly T _value;
        private readonly CsvFormat _format;
        private CsvRecord? _record;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CsvRecordWithValue(T value, CsvFormat format)
        {
            _record = null;
            _format = format;
            _value = value;
        }

        public CsvFormat Format
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _format;
        }
        
        public CsvRecord Record 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_record == null)
                {
                    _record = CsvRecord.From(_value, _format);
                }

                return _record;
            }
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
