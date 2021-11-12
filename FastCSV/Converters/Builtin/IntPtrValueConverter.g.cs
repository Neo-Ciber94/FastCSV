////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;
using System.Numerics;
using System.Net;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="IntPtr"/>.
    /// </summary>
    internal class IntPtrValueConverter : ICsvValueConverter<IntPtr>
    {
        public bool TrySerialize(IntPtr value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out IntPtr value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return IntPtr.TryParse(s.ToString(), out value!);
        }
    }
}
