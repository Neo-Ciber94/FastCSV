////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="UInt16"/>.
    /// </summary>
    internal class UShortValueConverter : ICsvValueConverter<UInt16>
    {
        public bool TrySerialize(UInt16 value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out UInt16 value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return UInt16.TryParse(s, out value!);
        }
    }
}
