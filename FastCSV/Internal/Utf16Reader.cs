using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Collections;
using FastCSV.Utils;

namespace FastCSV.Internal
{
    internal struct Utf16Reader : IBufferedReader<char>
    {
        private const int PositionDisposed = -1;

        private Utf8Reader _utf8Reader;
        private readonly Decoder? _decoder;
        private char[]? _arrayFromPool;
        private readonly int _maxCharsCount;
        private readonly Encoding _encoding;
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

        public Stream? Stream => _utf8Reader.Stream;

        public bool IsDone
        {
            get
            {
                if (_utf8Reader.IsDone && _charPos >= _charCount)
                {
                    return true;
                }

                return IsDisposed;
            }
        }

        public bool IsDisposed => _charPos == PositionDisposed;

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
                ReadOnlySpan<char> innerBuffer = FillBuffer();
                int totalRead = innerBuffer.Length;

                if (totalRead == 0)
                {
                    break;
                }

                int bufferFree = buffer.Length - written;
                int countToWrite = Math.Min(totalRead, bufferFree);
                innerBuffer.Slice(0, countToWrite).CopyTo(buffer[written..]);
                written += countToWrite;
                Consume(countToWrite);
            }

            return written;
        }

        public int ReadNext()
        {
            int value = Peek();

            if (value != -1)
            {
                Consume(1);
            }

            return value;
        }

        public int Peek()
        {
            ReadOnlySpan<char> buffer = FillBuffer();

            if (buffer.IsEmpty)
            {
                return -1;
            }

            return buffer[0];
        }

        public char[] ReadUntil(char value)
        {
            if (IsDone)
            {
                return Array.Empty<char>();
            }

            ArrayBuilder<char> builder = new ArrayBuilder<char>(_maxCharsCount);

            try
            {
                ReadUntil(ref builder, value);
                return builder.ToArray();
            }
            finally
            {
                builder.Dispose();
            }
        }

        public void ReadUntil(ref ArrayBuilder<char> builder, char value)
        {
            if (IsDone)
            {
                return;
            }

            while (true)
            {
                ReadOnlySpan<char> buffer = FillBuffer();

                if (buffer.Length == 0)
                {
                    break;
                }

                int index = buffer.IndexOf(value);
                int totalToConsume = index == -1 ? buffer.Length : index;

                if (index == -1)
                {
                    builder.AddRange(buffer);
                    Consume(totalToConsume);
                }
                else
                {
                    if (index > 0)
                    {
                        // We skip the found value
                        builder.AddRange(buffer.Slice(0, index));
                    }

                    Consume(totalToConsume + 1);
                    return;
                }
            }
        }

        public string? ReadLine()
        {
            if (IsDone)
            {
                return null;
            }

            ArrayBuilder<char> builder = new ArrayBuilder<char>(_maxCharsCount);

            try
            {
                ReadLine(ref builder);

                if (builder.Count == 0)
                {
                    return null;
                }

                return new string(builder.Span);
            }
            finally
            {
                builder.Dispose();
            }
        }

        public void ReadLine(ref ArrayBuilder<char> builder)
        {
            ReadUntil(ref builder, '\n');
            Span<char> buffer = builder.Span;

            if (buffer.Length > 0)
            {
                char c = buffer[^1];

                if (c == '\r')
                {
                    builder.RemoveLast();
                }
            }
        }

        public string? ReadToEnd()
        {
            if (IsDone)
            {
                return null;
            }

            ArrayBuilder<char> builder = new ArrayBuilder<char>(_maxCharsCount);

            try
            {
                ReadToEnd(ref builder);
                return new string(builder.Span);
            }
            finally
            {
                builder.Dispose();
            }
        }
       
        public void ReadToEnd(ref ArrayBuilder<char> builder)
        {
            if (IsDone)
            {
                return;
            }

            while (true)
            {
                var buffer = FillBuffer();

                if (buffer.Length == 0)
                {
                    break;
                }

                builder.AddRange(buffer);
                Consume(buffer.Length);
            }
        }

        public ReadOnlySpan<char> FillBuffer()
        {
            ThrowIfDisposed();

            if (_charPos < _charCount)
            {
                return _arrayFromPool.AsSpan(_charPos, _charCount - _charPos);
            }

            ReadOnlySpan<byte> byteBuffer = _utf8Reader.FillBuffer();

            if (byteBuffer.IsEmpty)
            {
                return ReadOnlySpan<char>.Empty;
            }

            int totalChars = _decoder!.GetChars(byteBuffer, _arrayFromPool, flush: false);
            _utf8Reader.Consume(byteBuffer.Length);

            if (totalChars == 0)
            {
                return ReadOnlySpan<char>.Empty;
            }

            _charPos = 0;
            _charCount = totalChars;
            return _arrayFromPool.AsSpan(0, totalChars);
        }

        public void Consume(int count)
        {
            ThrowIfDisposed();
            _charPos = Math.Min(count + _charPos, _charCount);
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

    internal struct LineEnumerator : IEnumerator<Memory<string>>
    {
        public Memory<string> Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
