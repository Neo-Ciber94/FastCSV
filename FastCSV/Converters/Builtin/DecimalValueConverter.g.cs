////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Decimal"/>.
    /// </summary>
    public class DecimalValueConverter : IValueConverter<System.Decimal>
    {
        public string? Read(System.Decimal value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.Decimal value)
        {
            return System.Decimal.TryParse(s, out value!);
        }
    }
}
