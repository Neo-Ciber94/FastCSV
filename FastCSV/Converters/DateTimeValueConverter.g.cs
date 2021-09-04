#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.DateTime"/>.
    /// </summary>
    public class DateTimeValueConverter : IValueConverter<System.DateTime>
    {
        public string? ToStringValue(System.DateTime value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.DateTime value)
        {
            return System.DateTime.TryParse(s, out value);
        }
    }
}

