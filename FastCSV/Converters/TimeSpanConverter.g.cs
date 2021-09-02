#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.TimeSpan"/>.
    /// </summary>
    public class TimeSpanConverter : IValueConverter<System.TimeSpan>
    {
        public string ToValue(System.TimeSpan value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.TimeSpan value)
        {
            return System.TimeSpan.TryParse(s, out value);
        }
    }
}

