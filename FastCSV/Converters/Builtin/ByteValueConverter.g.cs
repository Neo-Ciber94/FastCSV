////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Byte"/>.
    /// </summary>
    public class ByteValueConverter : IValueConverter<System.Byte>
    {
        public string? ConvertFrom(System.Byte value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Byte value)
        {
            return System.Byte.TryParse(s, out value!);
        }
    }
}
