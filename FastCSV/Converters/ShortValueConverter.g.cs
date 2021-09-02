#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Int16"/>.
    /// </summary>
    public class ShortValueConverter : IValueConverter<System.Int16>
    {
        public string ToValue(System.Int16 value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Int16 value)
        {
            return System.Int16.TryParse(s, out value);
        }
    }
}
