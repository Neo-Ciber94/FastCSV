////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Guid"/>.
    /// </summary>
    public class GuidValueConverter : ICsvCustomConverter<System.Guid>
    {
        public string? ConvertFrom(System.Guid value)
        {
            return value.ToString();
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out System.Guid value)
        {
            return System.Guid.TryParse(s, out value!);
        }
    }
}
