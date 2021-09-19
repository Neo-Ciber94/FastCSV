////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Byte"/>.
    /// </summary>
    public class ByteValueConverter : IValueConverter<System.Byte>
    {
        public string? ToStringValue(System.Byte value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Byte value)
        {
            return System.Byte.TryParse(s!, out value!);
        }
    }
}
