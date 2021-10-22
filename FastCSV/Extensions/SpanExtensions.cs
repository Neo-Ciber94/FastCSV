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

        public static Span<T> ReplaceAll<T>(this Span<T> span, T oldValue, T newValue) where T: IEquatable<T>
        {
            if (oldValue.Equals(newValue))
            {
                return span;
            }

            int length = span.Length;

            for (int i = 0; i < length; i++)
            {
                T value = span[i];

                if (value.Equals(oldValue))
                {
                    span[i] = newValue;
                }
            }

            return span;
        }

        public static Span<T> ReplaceAll<T>(this Span<T> span, T oldValue, ReadOnlySpan<T> newValue) where T : IEquatable<T>
        {
            return ReplaceAll(span, MemoryMarshal.CreateSpan(ref oldValue, 1), newValue);
        }

        public static Span<T> ReplaceAll<T>(this Span<T> span, ReadOnlySpan<T> oldValue, T newValue) where T : IEquatable<T>
        {
            return ReplaceAll(span, oldValue, MemoryMarshal.CreateSpan(ref newValue, 1));
        }

        public static Span<T> ReplaceAll<T>(this Span<T> span, ReadOnlySpan<T> oldValue, ReadOnlySpan<T> newValue) where T : IEquatable<T>
        {
            if (oldValue.SequenceEqual(newValue))
            {
                return span;
            }

            int totalToReplace = 0;
            int index = 0;

            while (true)
            {
                ReadOnlySpan<T> slice = span[index..];
                int pos = slice.IndexOf(oldValue);

                if (pos == -1)
                {
                    break;
                }

                totalToReplace += 1;
                index = pos + oldValue.Length;
            }

            if (totalToReplace == 0)
            {
                return span;
            }

            int newLength = span.Length + (totalToReplace * newValue.Length);

            if (newLength == span.Length)
            {
                TryReplaceAll(span, oldValue, newValue, span, out _);
                return span;
            }
            else
            {
                T[] arrayFromPool = ArrayPool<T>.Shared.Rent(newLength);
                TryReplaceAll(span, oldValue, newValue, arrayFromPool, out int totalWritten);
                Span<T> result = arrayFromPool.AsSpan(0, totalWritten);
                ArrayPool<T>.Shared.Return(arrayFromPool);
                return result;
            }
        }

        public static bool TryReplaceAll<T>(this ReadOnlySpan<T> span,
            ReadOnlySpan<T> oldValue,
            T newValue,
            Span<T> buffer,
            out int totalWritten) where T : IEquatable<T>
        {
            return TryReplaceAll(span, oldValue, MemoryMarshal.CreateSpan(ref newValue, 1), buffer, out totalWritten);
        }

        public static bool TryReplaceAll<T>(this ReadOnlySpan<T> span,
            T oldValue,
            ReadOnlySpan<T> newValue,
            Span<T> buffer,
            out int totalWritten) where T : IEquatable<T>
        {
            return TryReplaceAll(span, MemoryMarshal.CreateSpan(ref oldValue, 1), newValue, buffer, out totalWritten);
        }

        public static bool TryReplaceAll<T>(
            this ReadOnlySpan<T> span,
            ReadOnlySpan<T> oldValue,
            ReadOnlySpan<T> newValue,
            Span<T> buffer,
            out int totalWritten) where T : IEquatable<T>
        {
            totalWritten = 0;

            if (oldValue.Length == 0 || buffer.Length == 0 || newValue.Length > buffer.Length || oldValue.SequenceEqual(newValue))
            {
                return false;
            }

            using var indicesToReplace = new ValueList<int>(stackalloc int[128]);
            int index = 0;

            while (true)
            {
                ReadOnlySpan<T> slice = span[index..];
                int pos = slice.IndexOf(oldValue);

                if (pos == -1)
                {
                    break;
                }

                indicesToReplace.Add(pos + index);
                index += pos + oldValue.Length;
            }

            if (indicesToReplace.Length == 0)
            {
                return false;
            }

            int newValueLength = newValue.Length;
            int oldValueLength = oldValue.Length;

            int thisIdx = 0;
            int dstIdx = 0;

            for (int i = 0; i < indicesToReplace.Length; i++)
            {
                int indexToReplace = indicesToReplace[i];

                if ((indexToReplace + newValueLength) > buffer.Length)
                {
                    break;
                }

                int count = indexToReplace - thisIdx;

                if (count > 0)
                {
                    span.Slice(thisIdx, count).CopyTo(buffer[dstIdx..]);
                    thisIdx += count;
                    dstIdx += count;
                }

                if (newValueLength > 0)
                {
                    Span<T> dst = buffer[dstIdx..];
                    newValue.CopyTo(dst);

                    dstIdx += newValueLength;
                }

                thisIdx += oldValueLength;
            }

            // Copies the rest
            if (thisIdx < span.Length)
            {
                int bufferLeft = buffer.Length - dstIdx;
                int spanLeft = span.Length - thisIdx;
                int count = Math.Min(bufferLeft, spanLeft);
                span.Slice(thisIdx, count).CopyTo(buffer[dstIdx..]);
            }

            int diff = newValueLength - oldValueLength;
            int minLength = span.Length + (indicesToReplace.Length * diff);
            totalWritten = Math.Min(buffer.Length, minLength);
            return true;
        }
    }
}
