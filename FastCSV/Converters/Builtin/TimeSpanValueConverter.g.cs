////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;
using System.Numerics;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="TimeSpan"/>.
    /// </summary>
    internal class TimeSpanValueConverter : ICsvValueConverter<TimeSpan>
    {
        public bool TrySerialize(TimeSpan value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out TimeSpan value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return TimeSpan.TryParse(s, out value!);
        }
    }
}
