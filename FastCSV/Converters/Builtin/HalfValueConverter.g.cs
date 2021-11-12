////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Half"/>.
    /// </summary>
    internal class HalfValueConverter : ICsvValueConverter<Half>
    {
        public bool TrySerialize(Half value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Half value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Half.TryParse(s, out value!);
        }
    }
}
