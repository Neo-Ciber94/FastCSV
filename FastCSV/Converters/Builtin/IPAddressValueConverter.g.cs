////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Net.IPAddress"/>.
    /// </summary>
    public class IPAddressValueConverter : IValueConverter<System.Net.IPAddress>
    {
        public string? ConvertFrom(System.Net.IPAddress value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Net.IPAddress value)
        {
            return System.Net.IPAddress.TryParse(s, out value!);
        }
    }
}
