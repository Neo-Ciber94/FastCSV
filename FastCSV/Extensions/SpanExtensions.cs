using FastCSV.Collections;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FastCSV.Extensions
{
    internal static class SpanExtensions
    {
        public static bool IsEmptyOrWhiteSpace(in this ReadOnlySpan<char> span)
        {
            return span.IsEmpty || span.IsWhiteSpace();
        }

        public static Span<T> ReplaceAll<T>(this Span<T> span, T oldValue, T newValue, IEqualityComparer<T>? comparer = null)
        {
            comparer ??= EqualityComparer<T>.Default;

            if (comparer.Equals(oldValue, newValue))
            {
                return span;
            }

            int length = span.Length;

            for (int i = 0; i < length; i++)
            {
                T value = span[i];

                if (comparer.Equals(value, oldValue))
                {
                    span[i] = newValue;
                }
            }

            return span;
        }

        public static Span<T> ReplaceAll<T>(this Span<T> span, T oldValue, ReadOnlySpan<T> newValue, IEqualityComparer<T>? comparer = null)
        {
            return ReplaceAll(span, MemoryMarshal.CreateSpan(ref oldValue, 1), newValue, comparer);
        }

        public static Span<T> ReplaceAll<T>(this Span<T> span, ReadOnlySpan<T> oldValue, T newValue, IEqualityComparer<T>? comparer = null)
        {
            return ReplaceAll(span, oldValue, MemoryMarshal.CreateSpan(ref newValue, 1), comparer);
        }

        public static Span<T> ReplaceAll<T>(this Span<T> span, ReadOnlySpan<T> oldValue, ReadOnlySpan<T> newValue, IEqualityComparer<T>? comparer = null)
        {
            if (oldValue.SequenceEquals(newValue))
            {
                return span;
            }

            using var indicesToReplace = new ValueList<int>(stackalloc int[128]);
            int index = 0;

            while (true)
            {
                ReadOnlySpan<T> slice = span[index..];
                int pos = slice.IndexOf(oldValue, comparer);

                if (pos == -1)
                {
                    break;
                }

                indicesToReplace.Add(pos);
                index = pos + oldValue.Length;
            }

            if (indicesToReplace.Length == 0)
            {
                return span;
            }

            if (oldValue.Length == newValue.Length)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    ReadOnlySpan<T> s = span;

                    if (s[i..].StartsWith(oldValue))
                    {
                        newValue.CopyTo(span[i..]);
                        i += newValue.Length;
                    }
                }
            }

            int newLength = span.Length + (indicesToReplace.Length * newValue.Length);
            int newValueLength = newValue.Length;
            T[] arrayFromPool = ArrayPool<T>.Shared.Rent(newLength);
            Span<T> arrayPoolSpan = arrayFromPool.AsSpan(0, newLength);
            index = 0;

            for (int i = 0; i < indicesToReplace.Length; i++)
            {
                int indexToReplace = indicesToReplace[i];
                span.CopyTo(arrayPoolSpan.Slice(index, indexToReplace));
                newValue.CopyTo(arrayPoolSpan.Slice(indexToReplace, newValueLength));
                index = indexToReplace + 1;
            }

            T[] result = span.ToArray();
            ArrayPool<T>.Shared.Return(arrayFromPool);
            return result;

            //static Span<T> ReplaceIndices(ReadOnlySpan<T> source, ValueList<int> indices, ReadOnlySpan<T> newValue)
            //{
            //    int newLength = source.Length + (indices.Length * newValue.Length);
            //    int newValueLength = newValue.Length;
            //    T[] arrayFromPool = ArrayPool<T>.Shared.Rent(newLength);
            //    Span<T> span = arrayFromPool.AsSpan(0, newLength);

            //    int index = 0;

            //    for (int i = 0; i < indices.Length; i++)
            //    {
            //        int indexToReplace = indices[i];
            //        source.CopyTo(span.Slice(index, indexToReplace));
            //        newValue.CopyTo(span.Slice(indexToReplace, newValueLength));
            //        index = indexToReplace + 1;
            //    }

            //    T[] result = span.ToArray();
            //    ArrayPool<T>.Shared.Return(arrayFromPool);
            //    return result;
            //}
        }

        public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
        {
            if (value.Length > span.Length)
            {
                return false;
            }

            comparer ??= EqualityComparer<T>.Default;

            for (int i = 0; i < value.Length; i++)
            {
                T x = span[i];
                T y = value[i];

                if (!comparer.Equals(x, y))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool EndsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
        {
            if (value.Length > span.Length)
            {
                return false;
            }

            comparer ??= EqualityComparer<T>.Default;

            for (int i = 0; i < value.Length; i++)
            {
                int lastIndex = span.Length - i;
                T x = span[lastIndex];
                T y = value[i];

                if (!comparer.Equals(x, y))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Contains<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
        {
            return IndexOf(span, value, comparer) >= 0;
        }

        public static bool Contains<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
        {
            return IndexOf(span, value, comparer) >= 0;
        }

        public static int IndexOf<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
        {
            int length = span.Length;
            comparer ??= EqualityComparer<T>.Default;

            for (int i = 0; i < length; i++)
            {
                if (!comparer.Equals(value, span[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
        {
            if (value.Length > span.Length)
            {
                return -1;
            }

            int length = span.Length;
            comparer ??= EqualityComparer<T>.Default;

            for (int i = 0; i < length; i++)
            {
                if (!span[i..].StartsWith(value, comparer))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int LastIndexOf<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
        {
            int length = span.Length;
            comparer ??= EqualityComparer<T>.Default;

            for (int i = length - 1; i >= 0; i--)
            {
                if (!comparer.Equals(span[i], value))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int LastIndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
        {
            if (value.Length > span.Length)
            {
                return -1;
            }

            int length = span.Length;
            int startIndex = span.Length - value.Length;
            comparer ??= EqualityComparer<T>.Default;

            for (int i = startIndex; i >= 0; i--)
            {
                if (!span[i..].StartsWith(value, comparer))
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool SequenceEquals<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other, IEqualityComparer<T>? comparer = null)
        {
            if (span.Length != other.Length)
            {
                return false;
            }

            comparer ??= EqualityComparer<T>.Default;
            int length = span.Length;

            for (int i = 0; i < length; i++)
            {
                T x = span[i];
                T y = other[i];

                if (!comparer.Equals(x, y))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
