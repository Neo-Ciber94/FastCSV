////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable
        
using System;
using System.Numerics;

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="Version"/>.
    /// </summary>
    internal class VersionValueConverter : ICsvValueConverter<Version>
    {
        public bool TrySerialize(Version value, ref CsvSerializeState state)
        {
            state.Write(value.ToString());
            return true;
        }

        public bool TryDeserialize(out Version value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return Version.TryParse(s, out value!);
        }
    }
}
