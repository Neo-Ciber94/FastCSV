////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.Int32"/>.
    /// </summary>
    public class IntValueConverter : IValueConverter<System.Int32>
    {
        public string? Read(System.Int32 value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.Int32 value)
        {
            return System.Int32.TryParse(s, out value!);
        }
    }
}
