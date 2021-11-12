////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Int16"/>.
    /// </summary>
    internal class ShortValueConverter : ICsvValueConverter<Int16>
    {
        public bool TrySerialize(Int16 value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Int16 value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Int16.TryParse(s, out value!);
        }
    }
}
