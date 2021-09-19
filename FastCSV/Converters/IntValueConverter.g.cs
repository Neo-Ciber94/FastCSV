////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Int32"/>.
    /// </summary>
    public class IntValueConverter : IValueConverter<System.Int32>
    {
        public string? ToStringValue(System.Int32 value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Int32 value)
        {
            return System.Int32.TryParse(s!, out value!);
        }
    }
}
