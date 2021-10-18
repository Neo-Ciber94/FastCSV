////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Net.IPEndPoint"/>.
    /// </summary>
    public class IPEndPointValueConverter : ICsvCustomConverter<System.Net.IPEndPoint>
    {
        public string? ConvertFrom(System.Net.IPEndPoint value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Net.IPEndPoint value)
        {
            return System.Net.IPEndPoint.TryParse(s, out value!);
        }
    }
}
