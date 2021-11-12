////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="DateTimeOffset"/>.
    /// </summary>
    internal class DateTimeOffsetValueConverter : ICsvValueConverter<DateTimeOffset>
    {
        public bool TrySerialize(DateTimeOffset value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out DateTimeOffset value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return DateTimeOffset.TryParse(s, out value!);
        }
    }
}
