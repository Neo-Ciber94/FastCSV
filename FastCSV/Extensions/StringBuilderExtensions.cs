using System;
using System.Collections;
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

        public static StringBuilder Slice(this StringBuilder sb, int index)
        {
            return sb.Slice(index, sb.Length - index);
        }

        public static StringBuilder Slice(this StringBuilder sb, int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index cannot be negative but was {index}");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"Count cannot be negative {count}");
            }

            if ((index + count) > sb.Length)
            {
                throw new ArgumentOutOfRangeException($"Count is too large {count}");
            }

            int endIndex = index + count;
            sb.Remove(endIndex, sb.Length - endIndex);
            sb.Remove(0, index);
            return sb;
        }

        public static StringBuilder Slice(this StringBuilder sb, Index index)
        {
            int startIndex = index.GetOffset(sb.Length);
            return sb.Slice(startIndex, sb.Length - startIndex);
        }

        public static StringBuilder Slice(this StringBuilder sb, Range range)
        {

            var (index, count) = range.GetOffsetAndLength(sb.Length);
            return sb.Slice(index, count);
        }

        public static StringBuilder PadLeft(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            return sb.PadLeft(1, other);
        }

        public static StringBuilder PadLeft(this StringBuilder sb, int count, ReadOnlySpan<char> other)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException($"Count cannot be negative but was {count}");
            }

            if (count == 0)
            {
                return sb;
            }

            sb.EnsureCapacity(other.Length * count);

            for (int i = 0; i < count; i++)
            {
                sb.Insert(0, other);
            }

            return sb;
        }

        public static StringBuilder PadRight(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            return sb.PadRight(1, other);
        }

        public static StringBuilder PadRight(this StringBuilder sb, int count, ReadOnlySpan<char> other)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException($"Count cannot be negative but was {count}");
            }

            if (count == 0)
            {
                return sb;
            }

            sb.EnsureCapacity(other.Length * count);

            for (int i = 0; i < count; i++)
            {
                sb.Append(other);
            }

            return sb;
        }

        public static StringBuilder TrimStartOnce(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            if (sb.StartsWith(other))
            {
                return sb.Slice(other.Length);
            }

            return sb;
        }

        public static StringBuilder TrimEndOnce(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            if (sb.EndsWith(other))
            {
                return sb.Slice(0..^other.Length);
            }

            return sb;
        }

        public static StringBuilder TrimOnce(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            return sb.TrimStartOnce(other).TrimEndOnce(other);
        }

        public static int IndexOf(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            if (other.IsEmpty)
            {
                return -1;
            }

            int length = sb.Length;

            for (int i = 0; i < length; i++)
            {
                bool match = true;

                for (int j = 0; j < other.Length; j++)
                {
                    int index = i + j;

                    if (index >= length)
                    {
                        return -1;
                    }

                    if (sb[index] != other[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return i;
                }
            }

            return -1;
        }

        public static int LastIndexOf(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            if (other.IsEmpty)
            {
                return -1;
            }

            int length = sb.Length;
            int otherLength = other.Length;

            for (int i = length - otherLength; i >= 0; i--)
            {
                bool match = true;

                for (int j = 0; j < otherLength; j++)
                {
                    int index = i + j;

                    if (sb[index] != other[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool Contains(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            return sb.IndexOf(other) >= 0;
        }

        public static bool Contains(this StringBuilder sb, char c)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] != c)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool StartsWith(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            return sb.StartsWith(0, other);
        }

        public static bool StartsWith(this StringBuilder sb, int startIndex, ReadOnlySpan<char> other)
        {
            if (startIndex < 0 || startIndex > sb.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), $"Start index cannot be negative, equals or greater than StringBuilder Length: {startIndex}");
            }

            if (other.IsEmpty)
            {
                return false;
            }

            if (other.Length > sb.Length)
            {
                return false;
            }

            for (int i = startIndex; i < other.Length; i++)
            {
                if (sb[i] != other[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool EndsWith(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            return sb.EndsWith(sb.Length, other);
        }

        public static bool EndsWith(this StringBuilder sb, int startIndex, ReadOnlySpan<char> other)
        {
            if (startIndex < 0 || startIndex > sb.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), $"Start index cannot be negative, equals or greater than StringBuilder Length: {startIndex}");
            }

            if (other.IsEmpty)
            {
                return false;
            }

            int length = startIndex;

            if (other.Length > length)
            {
                return false;
            }

            for (int i = 0; i < other.Length; i++)
            {
                int index = length - other.Length + i;

                if (sb[index] != other[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool StartsWithIgnoreWhiteSpace(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            int startIndex = 0;

            while (startIndex < sb.Length)
            {
                char c = sb[startIndex];

                if (c == ' ')
                {
                    startIndex += 1;
                }
                else
                {
                    break;
                }
            }

            return sb.StartsWith(startIndex, other);
        }

        public static bool EndsWithIgnoreWhiteSpace(this StringBuilder sb, ReadOnlySpan<char> other)
        {
            int startIndex = sb.Length;

            while (startIndex > 0)
            {
                char c = sb[startIndex - 1];

                if (c == ' ')
                {
                    startIndex -= 1;
                }
                else
                {
                    break;
                }
            }

            return sb.EndsWith(startIndex, other);
        }

        public static StringBuilderEnumerator AsEnumerable(this StringBuilder sb)
        {
            return new StringBuilderEnumerator(sb);
        }
    }

    internal struct StringBuilderEnumerator : IEnumerator<char>, IEnumerable<char>
    {
        private readonly StringBuilder _sb;
        private int _index;

        public StringBuilderEnumerator(StringBuilder sb)
        {
            _sb = sb;
            _index = -1;
        }

        public char Current => _sb[_index];

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            _index += 1;

            if (_index >= _sb.Length)
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
            _index = -1;
        }

        public void Dispose()
        {
        }

        public StringBuilderEnumerator GetEnumerator() => this;

        IEnumerator<char> IEnumerable<char>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
