////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Int32"/>.
    /// </summary>
    internal class IntValueConverter : ICsvValueConverter<Int32>
    {
        public bool TrySerialize(Int32 value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Int32 value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Int32.TryParse(s, out value!);
        }
    }
}
