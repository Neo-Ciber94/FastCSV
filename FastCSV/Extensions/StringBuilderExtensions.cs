using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Utils
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder TrimStart(this StringBuilder sb, Predicate<char> condition)
        {
            int length = sb.Length;
            int i = 0;

            if (length == 0)
            {
                return sb;
            }

            do
            {
                char c = sb[i];

                if (!condition(c))
                {
                    break;
                }
                else
                {
                    i += 1;
                }
            }
            while (i < length);

            if (i > 0)
            {
                return sb.Remove(0, i);
            }

            return sb;
        }

        public static StringBuilder TrimStart(this StringBuilder sb) => TrimStart(sb, char.IsWhiteSpace);

        public static StringBuilder TrimEnd(this StringBuilder sb, Predicate<char> condition)
        {
            if (sb.Length == 0)
            {
                return sb;
            }

            int i = sb.Length - 1;

            do
            {
                char c = sb[i];

                if (!condition(c))
                {
                    break;
                }
                else
                {
                    i -= 1;
                }
            }
            while (i >= 0);

            if (i < sb.Length)
            {
                i += 1;
                return sb.Remove(i, sb.Length - i);
            }

            return sb;
        }

        public static StringBuilder TrimEnd(this StringBuilder sb) => TrimEnd(sb, char.IsWhiteSpace);

        public static StringBuilder Trim(this StringBuilder sb, Predicate<char> condition)
        {
            return sb.TrimStart(condition).TrimEnd(condition);
        }

        public static StringBuilder Trim(this StringBuilder sb) => Trim(sb, char.IsWhiteSpace);
    }
}
