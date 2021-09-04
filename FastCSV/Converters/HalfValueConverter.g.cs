#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Half"/>.
    /// </summary>
    public class HalfValueConverter : IValueConverter<System.Half>
    {
        public string? ToStringValue(System.Half value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Half value)
        {
            return System.Half.TryParse(s, out value);
        }
    }
}

