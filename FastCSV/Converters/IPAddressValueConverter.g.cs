#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Net.IPAddress"/>.
    /// </summary>
    public class IPAddressValueConverter : IValueConverter<System.Net.IPAddress>
    {
        public string ToValue(System.Net.IPAddress value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Net.IPAddress value)
        {
            return System.Net.IPAddress.TryParse(s, out value);
        }
    }
}
