////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.DateTimeOffset"/>.
    /// </summary>
    public class DateTimeOffsetValueConverter : IValueConverter<System.DateTimeOffset>
    {
        public string? Read(System.DateTimeOffset value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.DateTimeOffset value)
        {
            return System.DateTimeOffset.TryParse(s!, out value!);
        }
    }
}
