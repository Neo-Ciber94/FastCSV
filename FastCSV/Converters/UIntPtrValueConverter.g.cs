#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.UIntPtr"/>.
    /// </summary>
    public class UIntPtrValueConverter : IValueConverter<System.UIntPtr>
    {
        public string? ToValue(System.UIntPtr value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.UIntPtr value)
        {
            return System.UIntPtr.TryParse(s, out value);
        }
    }
}
