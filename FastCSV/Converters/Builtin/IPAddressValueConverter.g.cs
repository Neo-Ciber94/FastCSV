////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;
using System.Numerics;
using System.Net;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="IPAddress"/>.
    /// </summary>
    internal class IPAddressValueConverter : ICsvValueConverter<IPAddress>
    {
        public bool TrySerialize(IPAddress value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out IPAddress value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return IPAddress.TryParse(s, out value!);
        }
    }
}
