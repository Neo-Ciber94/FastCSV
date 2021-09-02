#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Byte"/>.
    /// </summary>
    public class ByteConverter : IValueConverter<System.Byte>
    {
        public string ToValue(System.Byte value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Byte value)
        {
            return System.Byte.TryParse(s, out value);
        }
    }
}

