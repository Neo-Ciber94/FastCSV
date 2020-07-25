
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

        public static bool IsNullOrBlank(this string? s)
        {
            if(s == null)
            {
                return true;
            }

            return IsBlank(s);
        }

        public static bool EnclosedWith(this string s, string value)
        {
            return s.StartsWith(value) && s.EndsWith(value);
        }

        public static bool EnclosedWith(this string s, char value)
        {
            return s.StartsWith(value) && s.EndsWith(value);
        }
    }
}
