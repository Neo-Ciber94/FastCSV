#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.IntPtr"/>.
    /// </summary>
    public class IntPtrValueConverter : IValueConverter<System.IntPtr>
    {
        public string? ToValue(System.IntPtr value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.IntPtr value)
        {
            return System.IntPtr.TryParse(s, out value);
        }
    }
}

