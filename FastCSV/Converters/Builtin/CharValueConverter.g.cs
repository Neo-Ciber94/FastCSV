////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Char"/>.
    /// </summary>
    public class CharValueConverter : IValueConverter<System.Char>
    {
        public string? ConvertFrom(System.Char value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Char value)
        {
            return System.Char.TryParse(s.ToString(), out value!);
        }
    }
}
