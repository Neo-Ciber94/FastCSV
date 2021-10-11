using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FastCSV.Utils;

namespace FastCSV.Extensions
{
    internal static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EnclosedWith(this string s, string value) => s.StartsWith(value) && s.EndsWith(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EnclosedWith(this string s, char value) => s.StartsWith(value) && s.EndsWith(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToStringOrEmpty<T>(this T obj)
        {
            return obj?.ToString() ?? string.Empty;
        }

        public static string IntoString<T>(this IEnumerable<T> enumerable, string separator = ",", bool encloseWithBrackets = true)
        {
            ValueStringBuilder sb = new ValueStringBuilder(stackalloc char[64]);
            var enumerator = enumerable.GetEnumerator();

            if (encloseWithBrackets)
            {
                sb.Append('[');
            }

            if(enumerator.MoveNext())
            {
                while (true)
                {
                    sb.Append(enumerator.Current);

                    if (enumerator.MoveNext())
                    {
                        sb.Append(separator);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (encloseWithBrackets)
            {
                sb.Append(']');
            }

            return sb.ToStringAndDispose();
        }

        public static string IntoString<T>(this Span<T> span, string separator = ",", bool encloseWithBrackets = true)
        {
            ValueStringBuilder sb = new ValueStringBuilder(stackalloc char[64]);
            var enumerator = span.GetEnumerator();

            if (encloseWithBrackets)
            {
                sb.Append('[');
            }

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    sb.Append(enumerator.Current);

                    if (enumerator.MoveNext())
                    {
                        sb.Append(separator);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (encloseWithBrackets)
            {
                sb.Append(']');
            }

            return sb.ToStringAndDispose();
        }

        public static string IntoString<T>(this ReadOnlySpan<T> span, string separator = ",", bool encloseWithBrackets = true)
        {
            ValueStringBuilder sb = new ValueStringBuilder(stackalloc char[64]);
            var enumerator = span.GetEnumerator();

            if (encloseWithBrackets)
            {
                sb.Append('[');
            }

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    sb.Append(enumerator.Current);

                    if (enumerator.MoveNext())
                    {
                        sb.Append(separator);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (encloseWithBrackets)
            {
                sb.Append(']');
            }

            return sb.ToStringAndDispose();
        }
    }
}
