////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Version"/>.
    /// </summary>
    public class VersionValueConverter : IValueConverter<System.Version>
    {
        public string? Read(System.Version value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Version value)
        {
            return System.Version.TryParse(s!, out value!);
        }
    }
}
