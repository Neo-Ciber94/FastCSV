using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FastCSV.Utils;

namespace FastCSV.Structs
{
    /// <summary>
    /// Provides a mechanism for read to csv documents.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class CsvReaderBuffered : IDisposable
    {
        private CsvBufferedReader _reader;
        private int _recordNumber = 0;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReaderBuffered"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public CsvReaderBuffered(string path) : this(path, CsvFormat.Default, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReaderBuffered"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        public CsvReaderBuffered(string path, bool hasHeader) : this(path, CsvFormat.Default, hasHeader) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReaderBuffered"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="format">The format.</param>
        public CsvReaderBuffered(string path, CsvFormat format) : this(path, format, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReaderBuffered"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="format">The format.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        public CsvReaderBuffered(string path, CsvFormat format, bool hasHeader) : this(new StreamReader(path), format, hasHeader) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReaderBuffered"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        public CsvReaderBuffered(StreamReader reader, CsvFormat? format = null, bool hasHeader = true)
        {
            _reader = new CsvBufferedReader(reader);
            Format = format ?? CsvFormat.Default;

            if (hasHeader)
            {
                string[]? values = _reader.ReadRecord(Format);
                if (values != null && values.Length > 0)
                {
                    Header = new CsvHeader(values, Format);
                    _recordNumber += 1;
                }
            }
        }

        internal CsvReaderBuffered(ReadOnlyMemory<char> data, CsvFormat? format = null, bool hasHeader = true)
        {
            _reader = new CsvBufferedReader(data);
            Format = format ?? CsvFormat.Default;

            if (hasHeader)
            {
                string[]? values = _reader.ReadRecord(Format);
                if (values != null && values.Length > 0)
                {
                    Header = new CsvHeader(values, Format);
                    _recordNumber += 1;
                }
            }
        }

        internal CsvReaderBuffered(Stream stream, CsvFormat? format = null, bool hasHeader = true)
        {
            _reader = new CsvBufferedReader(stream);
            Format = format ?? CsvFormat.Default;

            if (hasHeader)
            {
                string[]? values = _reader.ReadRecord(Format);
                if (values != null && values.Length > 0)
                {
                    Header = new CsvHeader(values, Format);
                    _recordNumber += 1;
                }
            }
        }

        /// <summary>
        /// Froms the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="format">The format.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        /// <returns></returns>
        public static CsvReaderBuffered FromStream(Stream stream, CsvFormat? format = null, bool hasHeader = true)
        {
            return new CsvReaderBuffered(stream, format, hasHeader);
        }

        public static CsvReaderBuffered FromString(string data, CsvFormat? format = null, bool hasHeader = true)
        {
            return new CsvReaderBuffered(data.AsMemory(), format, hasHeader);
        }

        /// <summary>
        /// Gets the format used by this reader.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public CsvFormat Format { get; }

        /// <summary>
        /// Gets the header of the document this reader is pointing to.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public CsvHeader? Header { get; }

        /// <summary>
        /// Gets the number of read records.
        /// </summary>
        /// <value>
        /// The number of records read.
        /// </value>
        public int RecordNumber => _recordNumber - 1;

        /// <summary>
        /// Gets a value indicating whether this reader csv has header.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this reader csv has header; otherwise, <c>false</c>.
        /// </value>
        public bool HasHeader => Header != null;

        /// <summary>
        /// Gets a value indicating whether this <see cref="CsvReaderBuffered"/> had read all the records.
        /// </summary>
        /// <value>
        ///   <c>true</c> if done; otherwise, <c>false</c>.
        /// </value>
        public bool IsDone => _reader.Peek() == null;

        /// <summary>
        /// Reads the next record.
        /// </summary>
        /// <returns>The next record or null is there is not more records</returns>
        public CsvRecord? Read()
        {
            ThrowIfDisposed();

            string[]? values = _reader.ReadRecord(Format);

            if (Format.IgnoreWhitespace && (values == null || values.Length == 0))
            {
                return null;
            }

            _recordNumber += 1;

            if (values == null)
            {
                return new CsvRecord(Header, Array.Empty<string>(), Format);
            }

            return new CsvRecord(Header, values, Format);
        }

        /// <summary>
        /// Reads the next record asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for cancelling this operation</param>
        /// <returns>The next record or null is there is not more records</returns>
        public ValueTask<CsvRecord?> ReadAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            string[]? values = _reader.ReadRecord(Format);

            if (Format.IgnoreWhitespace && (values == null || values.Length == 0))
            {
                return ValueTask.FromResult<CsvRecord?>(null);
            }

            _recordNumber += 1;

            if (values == null)
            {
                return ValueTask.FromResult<CsvRecord?>(new CsvRecord(Header, Array.Empty<string>(), Format));
            }

            return ValueTask.FromResult<CsvRecord?>(new CsvRecord(Header, values, Format));
        }

        /// <summary>
        /// Moves this reader to the start of the csv.
        /// </summary>
        /// <exception cref="InvalidOperationException">If is unable to move.</exception>
        public void Reset()
        {
            ThrowIfDisposed();

            if (!TryReset())
            {
                throw new InvalidOperationException("Cannot move to the start of the csv");
            }
        }

        /// <summary>
        /// Attemps to move this reader to the start of the csv.
        /// </summary>
        /// <returns><c>true</c> if moved to the start of the csv, otherwise false.</returns>
        public bool TryReset()
        {
            if (_disposed)
            {
                return false;
            }

            var stream = _reader.Stream;

            if (stream != null && stream.CanSeek)
            {
                stream.Position = 0;

                if (HasHeader)
                {
                    Read(); // Ignores the header
                }

                return true;
            }

            return false;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException($"{nameof(CsvReaderBuffered)} has been disposed");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    _reader.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Releases the resouces used by this reader.
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            ThrowIfDisposed();

            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~CsvReaderBuffered()
        {
            Dispose(true);
        }
    }
}
