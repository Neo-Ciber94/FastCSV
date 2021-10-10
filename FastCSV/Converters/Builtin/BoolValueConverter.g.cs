////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Boolean"/>.
    /// </summary>
    public class BoolValueConverter : IValueConverter<System.Boolean>
    {
        public string? ConvertFrom(System.Boolean value)
        {
            return value? "true": "false";
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Boolean value)
        {
            return System.Boolean.TryParse(s, out value!);
        }
    }
}
