////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Half"/>.
    /// </summary>
    public class HalfValueConverter : IValueConverter<System.Half>
    {
        public string? Read(System.Half value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.Half value)
        {
            return System.Half.TryParse(s, out value!);
        }
    }
}