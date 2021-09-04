
namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="string"/>, it just forwards the value.
    /// </summary>
    public class StringValueConverter : IValueConverter<string>
    {
        public string ToStringValue(string value)
        {
            return value;
        }

        public bool TryParse(string? s, out string value)
        {
            value = s?? string.Empty;
            return s != null;
        }
    }
}
