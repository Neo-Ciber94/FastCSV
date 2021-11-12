////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Double"/>.
    /// </summary>
    internal class DoubleValueConverter : ICsvValueConverter<Double>
    {
        public bool TrySerialize(Double value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Double value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Double.TryParse(s, out value!);
        }
    }
}
