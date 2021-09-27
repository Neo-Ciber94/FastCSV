////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Double"/>.
    /// </summary>
    public class DoubleValueConverter : IValueConverter<System.Double>
    {
        public string? Read(System.Double value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.Double value)
        {
            return System.Double.TryParse(s, out value!);
        }
    }
}
