////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Single"/>.
    /// </summary>
    internal class FloatValueConverter : ICsvValueConverter<Single>
    {
        public bool TrySerialize(Single value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Single value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Single.TryParse(s, out value!);
        }
    }
}
