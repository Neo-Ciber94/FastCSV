////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;
using System.Numerics;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="BigInteger"/>.
    /// </summary>
    internal class BigIntegerValueConverter : ICsvValueConverter<BigInteger>
    {
        public bool TrySerialize(BigInteger value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out BigInteger value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return BigInteger.TryParse(s, out value!);
        }
    }
}
