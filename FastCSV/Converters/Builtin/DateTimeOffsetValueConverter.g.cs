////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.DateTimeOffset"/>.
    /// </summary>
    public class DateTimeOffsetValueConverter : ICsvCustomConverter<System.DateTimeOffset>
    {
        public string? ConvertFrom(System.DateTimeOffset value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.DateTimeOffset value)
        {
            return System.DateTimeOffset.TryParse(s, out value!);
        }
    }
}
