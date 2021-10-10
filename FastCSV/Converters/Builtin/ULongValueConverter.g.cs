////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.UInt64"/>.
    /// </summary>
    public class ULongValueConverter : IValueConverter<System.UInt64>
    {
        public string? Read(System.UInt64 value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.UInt64 value)
        {
            return System.UInt64.TryParse(s, out value!);
        }
    }
}
