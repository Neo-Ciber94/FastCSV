////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Guid"/>.
    /// </summary>
    public class GuidValueConverter : IValueConverter<System.Guid>
    {
        public string? Read(System.Guid value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.Guid value)
        {
            return System.Guid.TryParse(s, out value!);
        }
    }
}
