////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Boolean"/>.
    /// </summary>
    internal class BoolValueConverter : ICsvValueConverter<Boolean>
    {
        public bool TrySerialize(Boolean value, ref CsvSerializeState state)
        {
            state.Write(value? "true": "false");
            return true;
        }

        public bool TryDeserialize(out Boolean value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Boolean.TryParse(s, out value!);
        }
    }
}
