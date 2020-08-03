﻿
using System;
using System.Collections.Generic;
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
