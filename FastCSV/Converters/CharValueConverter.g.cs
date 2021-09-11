////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Char"/>.
    /// </summary>
    public class CharValueConverter : IValueConverter<System.Char>
    {
        public string? ToStringValue(System.Char value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Char value)
        {
            return System.Char.TryParse(s, out value);
        }
    }
}
