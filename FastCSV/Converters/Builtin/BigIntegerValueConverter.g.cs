////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Numerics.BigInteger"/>.
    /// </summary>
    public class BigIntegerValueConverter : IValueConverter<System.Numerics.BigInteger>
    {
        public string? Read(System.Numerics.BigInteger value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.Numerics.BigInteger value)
        {
            return System.Numerics.BigInteger.TryParse(s, out value!);
        }
    }
}