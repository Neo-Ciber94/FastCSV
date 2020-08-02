
using System.Runtime.CompilerServices;

namespace FastCSV.Utils
{
    public static class StringExtensions
    {
        public static bool IsBlank(this string s)
        {
            if(s.Length == 0)
            {
                return true;
            }

            foreach(var c in s)
            {
                if (!char.IsWhiteSpace(c))
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrBlank(this string? s)
        {
            if(s == null)
            {
                return true;
            }

            return IsBlank(s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EnclosedWith(this string s, string value) => s.StartsWith(value) && s.EndsWith(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EnclosedWith(this string s, char value) => s.StartsWith(value) && s.EndsWith(value);
    }
}
