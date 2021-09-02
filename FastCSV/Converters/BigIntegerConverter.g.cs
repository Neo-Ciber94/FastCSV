#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref="System.Numerics.BigInteger"/>.
    /// </summary>
    public class BigIntegerConverter : IValueConverter<System.Numerics.BigInteger>
    {
        public string ToValue(System.Numerics.BigInteger value)
        {
            return value.ToString();
        }

        public bool TryParse(string? s, out System.Numerics.BigInteger value)
        {
            return System.Numerics.BigInteger.TryParse(s, out value);
        }
    }
}

