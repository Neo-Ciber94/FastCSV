////////////////// GENERATED CODE, DO NOT EDIT //////////////////

#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref="System.IntPtr"/>.
    /// </summary>
    public class IntPtrValueConverter : IValueConverter<System.IntPtr>
    {
        public string? Read(System.IntPtr value)
        {
            return value.ToString();
        }

        public bool TryParse(System.ReadOnlySpan<char> s, out System.IntPtr value)
        {
            return System.IntPtr.TryParse(s.ToString(), out value!);
        }
    }
}
