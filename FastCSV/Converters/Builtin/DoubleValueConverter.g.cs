////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Double"/>.
    /// </summary>
    public class DoubleValueConverter : IValueConverter<System.Double>
    {
        public string? ConvertFrom(System.Double value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Double value)
        {
            var result = System.Double.TryParse(s, out value!);
            return result;
        }
    }
}
