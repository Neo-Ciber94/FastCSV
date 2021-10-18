////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.UInt32"/>.
    /// </summary>
    public class UIntValueConverter : ICsvCustomConverter<System.UInt32>
    {
        public string? ConvertFrom(System.UInt32 value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.UInt32 value)
        {
            return System.UInt32.TryParse(s, out value!);
        }
    }
}
