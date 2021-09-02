#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.UInt64"/>.
    /// </summary>
    public class ULongConverter : IValueConverter<System.UInt64>
    {
        public string ToValue(System.UInt64 value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.UInt64 value)
        {
            return System.UInt64.TryParse(s, out value);
        }
    }
}

