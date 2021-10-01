////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.TimeSpan"/>.
    /// </summary>
    public class TimeSpanValueConverter : IValueConverter<System.TimeSpan>
    {
        public string? Read(System.TimeSpan value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.TimeSpan value)
        {
            return System.TimeSpan.TryParse(s, out value!);
        }
    }
}