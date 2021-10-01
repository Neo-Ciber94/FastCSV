////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.SByte"/>.
    /// </summary>
    public class SByteValueConverter : IValueConverter<System.SByte>
    {
        public string? Read(System.SByte value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.SByte value)
        {
            return System.SByte.TryParse(s, out value!);
        }
    }
}