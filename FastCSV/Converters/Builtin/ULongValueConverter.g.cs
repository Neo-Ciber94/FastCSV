////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="UInt64"/>.
    /// </summary>
    internal class ULongValueConverter : ICsvValueConverter<UInt64>
    {
        public bool TrySerialize(UInt64 value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out UInt64 value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return UInt64.TryParse(s, out value!);
        }
    }
}
