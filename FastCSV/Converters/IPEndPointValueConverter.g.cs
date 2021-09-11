#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Net.IPEndPoint"/>.
    /// </summary>
    public class IPEndPointValueConverter : IValueConverter<System.Net.IPEndPoint>
    {
        public string? ToStringValue(System.Net.IPEndPoint value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Net.IPEndPoint value)
        {
            return System.Net.IPEndPoint.TryParse(s, out value);
        }
    }
}
