////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Net.IPEndPoint"/>.
    /// </summary>
    public class IPEndPointValueConverter : IValueConverter<System.Net.IPEndPoint>
    {
        public string? Read(System.Net.IPEndPoint value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Net.IPEndPoint value)
        {
            return System.Net.IPEndPoint.TryParse(s!, out value!);
        }
    }
}
