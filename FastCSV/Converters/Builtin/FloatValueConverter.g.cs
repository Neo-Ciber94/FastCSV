////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Single"/>.
    /// </summary>
    public class FloatValueConverter : IValueConverter<System.Single>
    {
        public string? ConvertFrom(System.Single value)
        {
            return value.ToString("G");
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Single value)
        {
            return System.Single.TryParse(s, out value!);
        }
    }
}
