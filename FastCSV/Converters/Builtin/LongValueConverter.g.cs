////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Int64"/>.
    /// </summary>
    internal class LongValueConverter : ICsvValueConverter<Int64>
    {
        public bool TrySerialize(Int64 value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Int64 value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Int64.TryParse(s, out value!);
        }
    }
}
