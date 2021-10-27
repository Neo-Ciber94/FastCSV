using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Collections;

namespace FastCSV.Internal
{
    internal struct Utf16Reader : IBufferedReader<char>
    {
        private const int PositionDisposed = -1;
        private const int PositionDone = -2;

        private readonly Utf8Reader _utf8Reader;
        private char[]? _arrayFromPool;
        private readonly int _maxCharsCount;
        private readonly Encoding _encoding;
        private Decoder? _decoder;
        private int _charPos;
        private int _charCount;

        public Utf16Reader(Stream stream) : this(stream, Utf8Reader.DefaultBufferSize, false, Encoding.UTF8) { }

        public Utf16Reader(Stream stream, int bytesCapacity) : this(stream, bytesCapacity, false, Encoding.UTF8) { }

        public Utf16Reader(Stream stream, int bytesCapacity, bool leaveOpen = false, Encoding? encoding = null)
        {
            _utf8Reader = new Utf8Reader(stream, bytesCapacity, leaveOpen);
            _encoding = encoding ?? Encoding.UTF8;
            _decoder = _encoding.GetDecoder();
            _maxCharsCount = _encoding.GetMaxCharCount(bytesCapacity);
            _arrayFromPool = ArrayPool<char>.Shared.Rent(_maxCharsCount);
            _charPos = 0;
            _charCount = 0;
        }

        public bool IsDone => _charPos == PositionDone || IsDisposed;

        public bool IsDisposed => _charPos == PositionDone;

        public int Read(Span<char> buffer)
        {
            int destinationLength = buffer.Length;

            if (destinationLength == 0)
            {
                return 0;
            }

            int written = 0;

            while (written < buffer.Length)
            {
                Span<char> innerBuffer = FillBuffer();
                int totalRead = innerBuffer.Length;

                if (totalRead == 0)
                {
                    break;
                }

                int bufferFree = buffer.Length - written;
                int countToWrite = Math.Min(totalRead, bufferFree);
                innerBuffer.Slice(0, countToWrite).CopyTo(buffer.Slice(written));
                Consume(countToWrite);
            }

            return written;
        }

        public int ReadChar()
        {
            Span<char> buffer = FillBuffer();

            if (buffer.IsEmpty)
            {
                return -1;
            }

            char c = buffer[0];
            Consume(1);
            return c;
        }

        public char[] ReadUntil(char value)
        {
            if (IsDone)
            {
                return Array.Empty<char>();
            }

            using var builder = new ArrayBuilder<char>(32);

            while (true)
            {
                Span<char> buffer = FillBuffer();

                if (buffer.Length == 0)
                {
                    break;
                }

                int index = buffer.IndexOf(value);

                if (index == -1)
                {
                    builder.AddRange(buffer);
                }
                else
                {
                    if (index > 0)
                    {
                        // We skip the found value
                        builder.AddRange(buffer.Slice(0, index - 1));
                    }

                    break;
                }

                int totalToConsume = index == -1 ? buffer.Length : index;
                Consume(totalToConsume);
            }

            return builder.ToArray();
        }

        public Span<char> FillBuffer()
        {
            ThrowIfDisposed();

            if (_charCount > 0)
            {
                return _arrayFromPool.AsSpan(0, _charCount);
            }

            Span<byte> byteBuffer = _utf8Reader.FillBuffer();

            if (byteBuffer.IsEmpty)
            {
                _charPos = PositionDone;
                return Span<char>.Empty;
            }

            int totalChars = _decoder!.GetChars(byteBuffer, _arrayFromPool, flush: false);

            if (totalChars == 0)
            {
                return Span<char>.Empty;
            }

            _charPos = 0;
            _charCount = totalChars;
            return _arrayFromPool.AsSpan(0, totalChars);
        }

        public void Consume(int count)
        {
            ThrowIfDisposed();
            int countToConsume = Math.Min(count, _charCount - _charPos);
            _charPos += countToConsume;
        }

        public void DiscardBuffer()
        {
            _utf8Reader.DiscardBuffer();
            _charPos = 0;
            _charCount = 0;
        }

        public void Dispose()
        {
            _utf8Reader.Dispose();

            if (_arrayFromPool != null)
            {
                ArrayPool<char>.Shared.Return(_arrayFromPool);
                _arrayFromPool = null;
            }

            _charPos = PositionDisposed;
            _charCount = 0;
        }

        private void ThrowIfDisposed()
        {
            if (_charPos == PositionDisposed)
            {
                throw new ObjectDisposedException($"{GetType()} is already disposed");
            }
        }
    }
}
