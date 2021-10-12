using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace FastCSV.Utils
{
    public ref struct CsvBufferedReader
    {
        private static readonly Decoder Utf8Decoder = Encoding.UTF8.GetDecoder();

        private const int DefaultBufferCapacity = 512;

        private readonly char[]? _charBufferFromArrayPool;
        private readonly byte[]? _byteBufferFromArrayPool;

        private readonly Span<byte> _byteBuffer;
        private readonly ReadOnlySpan<char> _csvData;
        private readonly Stream? _stream;
        private readonly long _length;
        private readonly int _maxCharCount;
        private int _line;
        private int _pos;

        private int _charPos;
        private int _charsLen;

        public int Line => _line;

        public int Offset => _charPos;

        public long BytesLength => _line;

        public CsvBufferedReader(ReadOnlySpan<char> csvData)
        {
            _csvData = csvData;
            _length = Encoding.UTF8.GetByteCount(csvData);
            _stream = null;
            _pos = 0;
            _charPos = 0;
            _charsLen = 0;
            _line = 1;

            _maxCharCount = 0;
            _charBufferFromArrayPool = null;
            _byteBuffer = Span<byte>.Empty;
            _byteBufferFromArrayPool = null;
        }

        public CsvBufferedReader(Stream stream, Span<byte> buffer)
        {
            _csvData = ReadOnlySpan<char>.Empty;
            _byteBuffer = buffer;
            _byteBufferFromArrayPool = null;
            _length = stream.Length;
            _stream = stream;

            _pos = 0;
            _charPos = 0;
            _charsLen = 0;
            _line = 1;

            _maxCharCount = Encoding.UTF8.GetMaxCharCount(_byteBuffer.Length);
            _charBufferFromArrayPool = ArrayPool<char>.Shared.Rent(_maxCharCount);
        }

        public CsvBufferedReader(Stream stream, int capacity = DefaultBufferCapacity)
        {
            _csvData = ReadOnlySpan<char>.Empty;
            _length = stream.Length;
            _stream = stream;

            _pos = 0;
            _charPos = 0;
            _charsLen = 0;
            _line = 1;

            _byteBufferFromArrayPool = ArrayPool<byte>.Shared.Rent(capacity);
            _byteBuffer = _byteBufferFromArrayPool.AsSpan(0, capacity);

            _maxCharCount = Encoding.UTF8.GetMaxCharCount(_byteBuffer.Length);
            _charBufferFromArrayPool = ArrayPool<char>.Shared.Rent(_maxCharCount);
        }

        private ReadOnlySpan<char> GetCharBuffer()
        {
            int startIndex = _pos - _charsLen;

            if (_stream == null)
            {

                return _csvData.Slice(startIndex, _charsLen);
            }
            else
            {
                return _charBufferFromArrayPool.AsSpan(startIndex, _charsLen);
            }
        }

        public string[] NextRecord(CsvFormat format)
        {
            if (_charPos == _charsLen)
            {
                if (ReadBuffer() == 0)
                {
                    return Array.Empty<string>();
                }
            }

            throw new NotImplementedException();
        }

        public char? Read()
        {
            char? ch = Peek();

            if (ch == '\n')
            {
                _line += 1;
            }

            _charPos += 1;
            return ch;
        }

        public char? Peek()
        {
            if (_charPos == _charsLen)
            {
                if (ReadBuffer() == 0)
                {
                    return null;
                }
            }

            do
            {
                ReadOnlySpan<char> buffer = GetCharBuffer();

                while (_charPos < _charsLen)
                {
                    char ch = buffer[_charPos];
                    return ch;
                }
            }
            while (ReadBuffer() > 0);

            return null;
        }

        public string? ReadLine()
        {
            if (_charPos == _charsLen)
            {
                if (ReadBuffer() == 0)
                {
                    return null;
                }
            }

            StringBuilder? sb = null;

            do
            {
                while (_charPos < _charsLen)
                {
                    char? ch = Read();

                    if (ch == null)
                    {
                        goto ReturnString;
                    }

                    sb ??= StringBuilderCache.Acquire(512);

                    if (ch == '\n' || ch == '\r')
                    {
                        if (ch == '\r' && Peek() == '\n')
                        {
                            Read();
                        }

                        goto ReturnString;
                    }

                    sb.Append(ch);
                }
            }
            while (ReadBuffer() > 0);


        // Returns the resulting string, if any
        ReturnString:

            if (sb == null)
            {
                return null;
            }

            return StringBuilderCache.ToStringAndRelease(ref sb!);
        }

        private int ReadBuffer()
        {
            if (_pos >= _length)
            {
                return 0;
            }

            if (_stream != null)
            {
                int bytesRead = _stream.Read(_byteBuffer);

                if (bytesRead == 0)
                {
                    return 0;
                }

                int charsRead = Utf8Decoder.GetChars(_byteBuffer, _charBufferFromArrayPool.AsSpan(0, _maxCharCount), false);
                _pos += charsRead;
                _charsLen = charsRead;
                _charPos = 0;
                return charsRead;
            }
            else
            {
                int remainingChars = _csvData.Length - _pos;
                int length = Math.Min(DefaultBufferCapacity, remainingChars);

                _pos += length;
                _charsLen = length;
                _charPos = 0;
                return length;
            }
        }

        public void Dispose()
        {
            if (_byteBufferFromArrayPool != null)
            {
                ArrayPool<byte>.Shared.Return(_byteBufferFromArrayPool);
            }

            if (_charBufferFromArrayPool != null)
            {
                ArrayPool<char>.Shared.Return(_charBufferFromArrayPool);
            }
        }
    }
}
