////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Guid"/>.
    /// </summary>
    public class GuidValueConverter : IValueConverter<System.Guid>
    {
        public string? ToStringValue(System.Guid value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Guid value)
        {
            return System.Guid.TryParse(s!, out value!);
        }
    }
}
