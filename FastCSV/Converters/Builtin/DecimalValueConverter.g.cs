////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Decimal"/>.
    /// </summary>
    internal class DecimalValueConverter : ICsvValueConverter<Decimal>
    {
        public bool TrySerialize(Decimal value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Decimal value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Decimal.TryParse(s, out value!);
        }
    }
}
