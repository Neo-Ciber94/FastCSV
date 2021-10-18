////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Version"/>.
    /// </summary>
    public class VersionValueConverter : ICsvCustomConverter<System.Version>
    {
        public string? ConvertFrom(System.Version value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Version value)
        {
            return System.Version.TryParse(s, out value!);
        }
    }
}
