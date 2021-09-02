#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.SByte"/>.
    /// </summary>
    public class SByteValueConverter : IValueConverter<System.SByte>
    {
        public string ToValue(System.SByte value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.SByte value)
        {
            return System.SByte.TryParse(s, out value);
        }
    }
}

