#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Decimal"/>.
    /// </summary>
    public class DecimalValueConverter : IValueConverter<System.Decimal>
    {
        public string? ToStringValue(System.Decimal value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Decimal value)
        {
            return System.Decimal.TryParse(s, out value);
        }
    }
}

