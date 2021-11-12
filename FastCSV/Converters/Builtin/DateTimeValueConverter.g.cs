////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="DateTime"/>.
    /// </summary>
    internal class DateTimeValueConverter : ICsvValueConverter<DateTime>
    {
        public bool TrySerialize(DateTime value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out DateTime value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return DateTime.TryParse(s, out value!);
        }
    }
}
