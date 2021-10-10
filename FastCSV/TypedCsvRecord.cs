using System.Runtime.CompilerServices;
using FastCSV.Utils;

namespace FastCSV
{
    internal readonly struct TypedCsvRecord<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TypedCsvRecord(T value, CsvFormat format)
        {
            Record = CsvRecord.From(value, format); // FIXME: Lazy load record value
            Value = value;
        }
        
        public CsvRecord Record 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
        }

        public T Value 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (CsvRecord, T) ToTuple()
        {
            return (Record, Value);
        }
    }
}
