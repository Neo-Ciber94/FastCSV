////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;
using System.Numerics;
using System.Net;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="IPEndPoint"/>.
    /// </summary>
    internal class IPEndPointValueConverter : ICsvValueConverter<IPEndPoint>
    {
        public bool TrySerialize(IPEndPoint value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out IPEndPoint value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return IPEndPoint.TryParse(s, out value!);
        }
    }
}
