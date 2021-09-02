#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Single"/>.
    /// </summary>
    public class FloatValueConverter : IValueConverter<System.Single>
    {
        public string? ToValue(System.Single value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Single value)
        {
            return System.Single.TryParse(s, out value);
        }
    }
}

