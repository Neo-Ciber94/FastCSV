////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.SByte"/>.
    /// </summary>
    public class SByteValueConverter : ICsvCustomConverter<System.SByte>
    {
        public string? ConvertFrom(System.SByte value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.SByte value)
        {
            return System.SByte.TryParse(s, out value!);
        }
    }
}
