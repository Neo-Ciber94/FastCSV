////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.UInt16"/>.
    /// </summary>
    public class UShortValueConverter : IValueConverter<System.UInt16>
    {
        public string? Read(System.UInt16 value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.UInt16 value)
        {
            return System.UInt16.TryParse(s, out value!);
        }
    }
}
