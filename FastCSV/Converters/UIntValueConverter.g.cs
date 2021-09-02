#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.UInt32"/>.
    /// </summary>
    public class UIntValueConverter : IValueConverter<System.UInt32>
    {
        public string ToValue(System.UInt32 value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.UInt32 value)
        {
            return System.UInt32.TryParse(s, out value);
        }
    }
}

