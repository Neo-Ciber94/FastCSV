////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Byte"/>.
    /// </summary>
    internal class ByteValueConverter : ICsvValueConverter<Byte>
    {
        public bool TrySerialize(Byte value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Byte value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Byte.TryParse(s, out value!);
        }
    }
}
