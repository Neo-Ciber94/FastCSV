////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.DateTime"/>.
    /// </summary>
    public class DateTimeValueConverter : ICsvCustomConverter<System.DateTime>
    {
        public string? ConvertFrom(System.DateTime value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.DateTime value)
        {
            return System.DateTime.TryParse(s, out value!);
        }
    }
}
