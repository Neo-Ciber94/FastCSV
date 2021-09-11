////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Int64"/>.
    /// </summary>
    public class LongValueConverter : IValueConverter<System.Int64>
    {
        public string? ToStringValue(System.Int64 value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Int64 value)
        {
            return System.Int64.TryParse(s, out value);
        }
    }
}
