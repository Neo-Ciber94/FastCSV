#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.UInt16"/>.
    /// </summary>
    public class UShortValueConverter : IValueConverter<System.UInt16>
    {
        public string ToValue(System.UInt16 value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.UInt16 value)
        {
            return System.UInt16.TryParse(s, out value);
        }
    }
}

