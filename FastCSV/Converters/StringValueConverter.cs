using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="string"/>, it just forwards the value.
    /// </summary>
    public class StringValueConverter : IValueConverter<string>
    {
        public string ConvertFrom(string value)
        {
            return value;
        }

        public bool ConvertTo(ReadOnlySpan<char> s, out string value)
        {
            value = s.ToString();
            return true;
        }
    }
}
