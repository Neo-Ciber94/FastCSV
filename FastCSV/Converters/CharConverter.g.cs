#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Char"/>.
    /// </summary>
    public class CharConverter : IValueConverter<System.Char>
    {
        public string ToValue(System.Char value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Char value)
        {
            return System.Char.TryParse(s, out value);
        }
    }
}

