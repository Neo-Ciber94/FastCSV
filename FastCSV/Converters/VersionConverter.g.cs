#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Version"/>.
    /// </summary>
    public class VersionConverter : IValueConverter<System.Version>
    {
        public string ToValue(System.Version value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Version value)
        {
            return System.Version.TryParse(s, out value);
        }
    }
}

