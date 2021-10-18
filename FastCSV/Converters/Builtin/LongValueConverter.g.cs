////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Int64"/>.
    /// </summary>
    public class LongValueConverter : ICsvCustomConverter<System.Int64>
    {
        public string? ConvertFrom(System.Int64 value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Int64 value)
        {
            return System.Int64.TryParse(s, out value!);
        }
    }
}
