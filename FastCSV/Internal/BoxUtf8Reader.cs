using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using FastCSV.Collections;

namespace FastCSV.Internal
{
    internal class BoxUtf8Reader : IBufferedReader<byte>
    {
        internal const int DefaultBufferSize = 256;
        private const int PositionDisposed = -1;

        private byte[]? _arrayFromPool;
        private Stream? _stream;
        private int _pos;
        private int _capacity;

        private readonly bool _leaveOpen;

        public BoxUtf8Reader(Stream stream) : this(stream, DefaultBufferSize) { }

        public BoxUtf8Reader(Stream stream, int capacity, bool leaveOpen = false)
        {
            _arrayFromPool = ArrayPool<byte>.Shared.Rent(capacity);
            _stream = stream;
            _leaveOpen = leaveOpen;
            _pos = 0;
            _capacity = 0;
        }

        public Stream? Stream => _stream;

        public bool IsDone
        {
            get
            {
                if (_stream != null && _stream.Position == _stream.Length && _pos >= _capacity)
                {
                    return true;
                }

                return IsDisposed;
            }
        }

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
                ReadOnlySpan<byte> innerBuffer = FillBuffer();
                int totalRead = innerBuffer.Length;

                if (totalRead == 0)
                {
                    break;
                }

                int bufferFree = buffer.Length - written;
                int countToWrite = Math.Min(totalRead, bufferFree);
                innerBuffer.Slice(0, countToWrite).CopyTo(buffer.Slice(written));
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
            ReadOnlySpan<byte> buffer = FillBuffer();

            if (buffer.Length == 0)
            {
                return -1;
            }

            return buffer[0];
        }

        public byte[] ReadUntil(byte value)
        {
            if (IsDone)
            {
                return Array.Empty<byte>();
            }

            ArrayBuilder<byte> builder = new(64);

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

        public void ReadUntil(ref ArrayBuilder<byte> builder, byte value)
        {
            if (IsDone)
            {
                return;
            }

            while (true)
            {
                ReadOnlySpan<byte> buffer = FillBuffer();

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

        public ReadOnlySpan<byte> FillBuffer()
        {
            ThrowIfDisposed();

            if (_pos < _capacity)
            {
                return _arrayFromPool!.AsSpan(_pos, _capacity - _pos);
            }

            int totalRead = _stream!.Read(_arrayFromPool);

            if (totalRead == 0)
            {
                return ReadOnlySpan<byte>.Empty;
            }

            _pos = 0;
            _capacity = totalRead;

            return _arrayFromPool!.AsSpan(0, totalRead);
        }

        public void Consume(int count)
        {
            ThrowIfDisposed();
            _pos = Math.Min(count + _pos, _capacity);
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
