////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;
using System.Numerics;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Guid"/>.
    /// </summary>
    internal class GuidValueConverter : ICsvValueConverter<Guid>
    {
        public bool TrySerialize(Guid value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Guid value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Guid.TryParse(s, out value!);
        }
    }
}
