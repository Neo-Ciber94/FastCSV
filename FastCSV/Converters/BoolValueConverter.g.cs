#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Boolean"/>.
    /// </summary>
    public class BoolValueConverter : IValueConverter<System.Boolean>
    {
        public string? ToStringValue(System.Boolean value)
        {
            return value? "true": "false";
        }

        public bool TryParse(string? s, out System.Boolean value)
        {
            return System.Boolean.TryParse(s, out value);
        }
    }
}

