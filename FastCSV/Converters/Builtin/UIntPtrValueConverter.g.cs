////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;
using System.Numerics;
using System.Net;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="UIntPtr"/>.
    /// </summary>
    internal class UIntPtrValueConverter : ICsvValueConverter<UIntPtr>
    {
        public bool TrySerialize(UIntPtr value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out UIntPtr value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return UIntPtr.TryParse(s.ToString(), out value!);
        }
    }
}
