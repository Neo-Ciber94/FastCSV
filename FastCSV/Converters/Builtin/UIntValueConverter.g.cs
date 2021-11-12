////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="UInt32"/>.
    /// </summary>
    internal class UIntValueConverter : ICsvValueConverter<UInt32>
    {
        public bool TrySerialize(UInt32 value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out UInt32 value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return UInt32.TryParse(s, out value!);
        }
    }
}
