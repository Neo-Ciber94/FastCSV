////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Single"/>.
    /// </summary>
    public class FloatValueConverter : IValueConverter<System.Single>
    {
        public string? ToStringValue(System.Single value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Single value)
        {
            return System.Single.TryParse(s, out value);
        }
    }
}
