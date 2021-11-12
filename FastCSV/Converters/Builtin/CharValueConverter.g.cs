////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Char"/>.
    /// </summary>
    internal class CharValueConverter : ICsvValueConverter<Char>
    {
        public bool TrySerialize(Char value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Char value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Char.TryParse(s.ToString(), out value!);
        }
    }
}
