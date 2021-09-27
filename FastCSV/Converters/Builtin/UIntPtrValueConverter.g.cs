////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.UIntPtr"/>.
    /// </summary>
    public class UIntPtrValueConverter : IValueConverter<System.UIntPtr>
    {
        public string? Read(System.UIntPtr value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.UIntPtr value)
        {
            return System.UIntPtr.TryParse(s.ToString(), out value!);
        }
    }
}
