////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="SByte"/>.
    /// </summary>
    internal class SByteValueConverter : ICsvValueConverter<SByte>
    {
        public bool TrySerialize(SByte value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out SByte value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return SByte.TryParse(s, out value!);
        }
    }
}
