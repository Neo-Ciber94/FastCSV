using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace FastCSV.Utils
{
    public static class StreamHelper
    {
        /// <summary>
        /// Gets a <see cref="MemoryStream"/> containing the specified <see cref="ReadOnlySpan{char}"/> data.
        /// </summary>
        /// <param name="s">The data.</param>
        /// <param name="writable">Whether the returned <see cref="MemoryStream"/> should be writable.</param>
        /// <returns>A stream containing the specified data.</returns>
        public static Stream CreateStreamFromString(ReadOnlySpan<char> s, bool writable = false)
        {
            if (s.IsEmpty)
            {
                return Stream.Null;
            }

            int totalBytes = Encoding.UTF8.GetByteCount(s);
            byte[] arrayFromPool = ArrayPool<byte>.Shared.Rent(totalBytes);

            Span<byte> span = arrayFromPool.AsSpan(0, totalBytes);
            Encoding.UTF8.GetBytes(s, span);
            byte[] byteArray = span.ToArray();

            // Returns the array to the pool
            ArrayPool<byte>.Shared.Return(arrayFromPool);

            if (writable)
            {
                var memoryStream = new MemoryStream(capacity: byteArray.Length);
                memoryStream.Write(byteArray, 0, byteArray.Length);
                return memoryStream;
            }
            else
            {
                return new MemoryStream(byteArray);
            }
        }
    }
}
