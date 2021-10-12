using FastCSV.Collections;
using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace FastCSV.Utils
{
    public struct CsvBufferedReader : IDisposable
    {
        private static readonly Decoder Utf8Decoder = Encoding.UTF8.GetDecoder();
        public const int DefaultBufferCapacity = 256;

        private readonly char[]? _charBufferFromArrayPool;
        private readonly byte[]? _byteBufferFromArrayPool;

        private readonly Memory<byte> _byteBuffer;
        private readonly ReadOnlyMemory<char> _csvData;
        private readonly object? _stream;
        private readonly long _length;
        private readonly int _maxCharCount;
        private int _line;
        private int _pos;

        private int _charPos;
        private int _charsLen;

        public int Line => _line;

        public int Offset => _charPos;

        public CsvBufferedReader(ReadOnlyMemory<char> csvData)
        {
            _csvData = csvData;
            _length = csvData.Length;
            _stream = null;
            _pos = 0;
            _charPos = 0;
            _charsLen = 0;
            _line = 1;

            _maxCharCount = 0;
            _charBufferFromArrayPool = null;
            _byteBuffer = Memory<byte>.Empty;
            _byteBufferFromArrayPool = null;
        }

        public CsvBufferedReader(Stream stream, Memory<byte> buffer)
        {
            _csvData = ReadOnlyMemory<char>.Empty;
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
            _csvData = ReadOnlyMemory<char>.Empty;
            _length = stream.Length;
            _stream = stream;

            _pos = 0;
            _charPos = 0;
            _charsLen = 0;
            _line = 1;

            _byteBufferFromArrayPool = ArrayPool<byte>.Shared.Rent(capacity);
            _byteBuffer = _byteBufferFromArrayPool.AsMemory(0, capacity);

            _maxCharCount = Encoding.UTF8.GetMaxCharCount(_byteBuffer.Length);
            _charBufferFromArrayPool = ArrayPool<char>.Shared.Rent(_maxCharCount);
        }

        public CsvBufferedReader(StreamReader stream, int capacity = DefaultBufferCapacity)
        {
            _csvData = ReadOnlyMemory<char>.Empty;
            _length = stream.BaseStream.Length;
            _stream = stream;

            _pos = 0;
            _charPos = 0;
            _charsLen = 0;
            _line = 1;

            _byteBufferFromArrayPool = ArrayPool<byte>.Shared.Rent(capacity);
            _byteBuffer = _byteBufferFromArrayPool.AsMemory(0, capacity);

            _maxCharCount = Encoding.UTF8.GetMaxCharCount(_byteBuffer.Length);
            _charBufferFromArrayPool = ArrayPool<char>.Shared.Rent(_maxCharCount);
        }

        private ReadOnlySpan<char> GetCharBuffer()
        {
            int startIndex = _pos - _charsLen;

            if (_stream == null)
            {

                return _csvData.Span.Slice(startIndex, _charsLen);
            }
            else
            {
                return _charBufferFromArrayPool.AsSpan(startIndex, _charsLen);
            }
        }

        #region Synchronous methods
        public string[] ReadRecord(CsvFormat format)
        {
            if (_charPos == _charsLen)
            {
                if (ReadBuffer() == 0)
                {
                    return Array.Empty<string>();
                }
            }

            using ValueStringBuilder stringBuilder = new(stackalloc char[512]);
            using ArrayBuilder<string> records = new(10);

            char delimiter = format.Delimiter;
            char quote = format.Quote;
            QuoteStyle style = format.Style;
            bool hasQuote = false;

            while (true)
            {
                string? line = ReadLine();

                if (line == null)
                {
                    break;
                }

                // Ignore empty entries if the format don't allow whitespaces
                if (format.IgnoreWhitespace && string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Convert the CharEnumerator into an IIterator
                // which allow to inspect the next elements
                SpanIterator<char> enumerator = new(line);

                while (enumerator.MoveNext())
                {
                    char nextChar = enumerator.Current;

                    // We ignore any CR (carrier return) or LF (line-break)
                    if (!hasQuote && (nextChar == '\r' || nextChar == '\n'))
                    {
                        continue;
                    }

                    if (nextChar == delimiter)
                    {
                        if (hasQuote)
                        {
                            stringBuilder.Append(nextChar);
                        }
                        else
                        {
                            // Gets the current field and trim the whitespaces if required by the format
                            string field = stringBuilder.ToString();

                            if (format.IgnoreWhitespace)
                            {
                                field = field.Trim();
                            }

                            records.Add(field);
                            stringBuilder.Clear();
                        }
                    }
                    else if (nextChar == quote)
                    {
                        if (hasQuote)
                        {
                            // If the next char is a quote, the current is an escape so ignore it
                            // and append the next char
                            if (enumerator.Peek.Contains(quote) && enumerator.MoveNext())
                            {
                                if (style != QuoteStyle.Never)
                                {
                                    stringBuilder.Append(enumerator.Current);
                                }
                            }
                            else
                            {
                                switch (style)
                                {
                                    case QuoteStyle.Always:
                                        stringBuilder.Append(quote);
                                        break;
                                    case QuoteStyle.Never:
                                        break;
                                    case QuoteStyle.WhenNeeded:
                                        if (!enumerator.HasNext() || !enumerator.Peek.Contains(delimiter))
                                        {
                                            stringBuilder.Append(quote);
                                        }
                                        break;
                                }

                                hasQuote = false;
                            }
                        }
                        else
                        {
                            switch (style)
                            {
                                case QuoteStyle.Always:
                                    stringBuilder.Append(quote);
                                    break;
                                case QuoteStyle.Never:
                                    break;
                                case QuoteStyle.WhenNeeded:
                                    if (stringBuilder.Length > 0)
                                    {
                                        stringBuilder.Append(quote);
                                    }
                                    break;
                            }

                            hasQuote = true;
                        }
                    }
                    else
                    {
                        stringBuilder.Append(nextChar);
                    }
                }

                // Add the last record value
                records.Add(stringBuilder.ToString());

                // Exit if we aren't in a quote
                if (!hasQuote)
                {
                    break;
                }
            }

            if (hasQuote)
            {
                throw new CsvFormatException($"Quote wasn't closed. line '{Line}', offset '{Offset}'");
            }

            return records.ToArray();
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

                    sb ??= StringBuilderCache.Acquire(128);

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
                int charsRead = ReadFromStream(_charBufferFromArrayPool.AsSpan(0, _maxCharCount));
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

        private int ReadFromStream(Span<char> buffer)
        {
            if (_stream is TextReader reader)
            {
                return reader.Read(buffer);
            }

            if (_stream is Stream s)
            {
                Span<byte> byteBuffer = _byteBuffer.Span;
                int bytesRead = s.Read(byteBuffer);

                if (bytesRead == 0)
                {
                    return 0;
                }

                Utf8Decoder.GetChars(byteBuffer, buffer, false);
                return Utf8Decoder.GetCharCount(byteBuffer.Slice(0, bytesRead), false);
            }

            throw new InvalidOperationException($"Unable to read data from the stream: {_stream}");
        }
        #endregion

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
