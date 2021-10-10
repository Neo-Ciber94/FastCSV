////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Int16"/>.
    /// </summary>
    public class ShortValueConverter : IValueConverter<System.Int16>
    {
        public string? ConvertFrom(System.Int16 value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Int16 value)
        {
            return System.Int16.TryParse(s, out value!);
        }
    }
}
