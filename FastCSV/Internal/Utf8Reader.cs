using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Collections;

namespace FastCSV.Internal
{
    internal struct Utf8Reader : IBufferedReader<byte>
    {
        internal const int DefaultBufferSize = 256;
        private const int PositionDisposed = -1;
        private const int PositionDone = -2;

        private byte[]? _arrayFromPool;
        private Stream? _stream;
        private int _pos;
        private int _capacity;

        private readonly bool _leaveOpen;

        public Utf8Reader(Stream stream) : this(stream, DefaultBufferSize) { }

        public Utf8Reader(Stream stream, int capacity, bool leaveOpen = false)
        {
            _arrayFromPool = ArrayPool<byte>.Shared.Rent(capacity);
            _stream = stream;
            _leaveOpen = leaveOpen;
            _pos = 0;
            _capacity = capacity;
        }

        public bool IsDone => _pos == PositionDone || IsDisposed;

        public bool IsDisposed => _pos == PositionDisposed;

        public int Read(Span<byte> buffer)
        {
            int destinationLength = buffer.Length;

            if (destinationLength == 0)
            {
                return 0;
            }

            int written = 0;

            while (written < buffer.Length)
            {
                Span<byte> innerBuffer = FillBuffer();
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

        public int ReadByte()
        {
            Span<byte> buffer = FillBuffer();

            if (buffer.Length == 0)
            {
                return -1;
            }

            int value = buffer[0];
            Consume(1);
            return value;
        }

        public byte[] ReadUntil(byte value)
        {
            if (IsDone)
            {
                return Array.Empty<byte>();
            }

            using var builder = new ArrayBuilder<byte>(32);

            while (true)
            {
                Span<byte> buffer = FillBuffer();

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

        public Span<byte> FillBuffer()
        {
            ThrowIfDisposed();

            if (_capacity > 0)
            {
                return _arrayFromPool!.AsSpan(0, _capacity);
            }

            int bytesRead = _stream!.Read(_arrayFromPool);
            
            if (bytesRead == 0)
            {
                _pos = PositionDone;
                return Span<byte>.Empty;
            }

            _pos = 0;
            _capacity = bytesRead;

            return _arrayFromPool!.AsSpan(0, bytesRead);
        }

        public void Consume(int count)
        {
            ThrowIfDisposed();

            if (_pos < _capacity)
            {
                int actualBytesToConsume = Math.Min(count, _capacity  - _pos);
                _pos += actualBytesToConsume;
            }
        }

        public void DiscardBuffer()
        {
            ThrowIfDisposed();
            _pos = 0;
            _capacity = 0;
        }

        public void Dispose()
        {
            ThrowIfDisposed();

            if (_stream != null && _leaveOpen == false)
            {
                _stream.Dispose();
            }

            if (_arrayFromPool != null)
            {
                ArrayPool<byte>.Shared.Return(_arrayFromPool);
            }

            _pos = PositionDisposed;
            _stream = null;
            _arrayFromPool = null;
        }

        private void ThrowIfDisposed()
        {
            if (_pos == PositionDisposed)
            {
                throw new ObjectDisposedException($"{GetType()} is already disposed");
            }
        }
    }
}
