using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FastCSV
{
    /// <summary>
    /// Provides a mechanism for read to csv documents.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class CsvReader : IDisposable
    {
        private CsvParser _reader;
        private int _recordNumber = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public CsvReader(string path) : this(path, CsvFormat.Default, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        public CsvReader(string path, bool hasHeader) : this(path, CsvFormat.Default, hasHeader) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="format">The format.</param>
        public CsvReader(string path, CsvFormat format) : this(path, format, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="format">The format.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        public CsvReader(string path, CsvFormat format, bool hasHeader) : this(new StreamReader(path), format, hasHeader) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        public CsvReader(StreamReader reader, CsvFormat? format = null, bool hasHeader = true)
        {
            format ??= CsvFormat.Default;
            _reader = new CsvParser(reader, format);
            Format = format;

            if (hasHeader)
            {
                string[]? values = _reader.ParseNext();
                if (values != null && values.Length > 0)
                {
                    Header = new CsvHeader(values, Format);
                    _recordNumber += 1;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        /// <param name="leaveOpen">Whether if leave the stream open after dispose, default is false.</param>
        public CsvReader(Stream stream, CsvFormat? format = null, bool hasHeader = true, bool leaveOpen = false) 
            : this(new StreamReader(stream, leaveOpen: leaveOpen), format, hasHeader) { }

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
        /// Gets a value indicating whether this <see cref="CsvReader"/> had read all the records.
        /// </summary>
        /// <value>
        ///   <c>true</c> if done; otherwise, <c>false</c>.
        /// </value>
        public bool IsDone => _reader.IsDone;

        /// <summary>
        /// Reads the next record.
        /// </summary>
        /// <param name="overrideFormat">Overrides the format used for this reader to read the records.</param>
        /// <returns>The next record or null is there is not more records</returns>
        public CsvRecord? Read(CsvFormat? overrideFormat = null)
        {
            ThrowIfDisposed();

            string[]? values = _reader.ParseNext();

            if (values == null || values.Length == 0)
            {
                return null;
            }

            _recordNumber += 1;

            CsvFormat format = overrideFormat ?? Format;
            CsvHeader? header = overrideFormat == null ? Header : Header?.WithFormat(format);
            values ??= Array.Empty<string>();

            return new CsvRecord(header, values, format);
        }

        /// <summary>
        /// Reads the next record asynchronously.
        /// </summary>
        /// <param name="overrideFormat">Overrides the format used for this reader to read the records.</param>
        /// <param name="cancellationToken">A cancellation token for cancelling this operation</param>
        /// <returns>The next record or null is there is not more records</returns>
        public async ValueTask<CsvRecord?> ReadAsync(CsvFormat? overrideFormat = null, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            string[]? values = await _reader.ParseNextAsync(cancellationToken);

            if (values == null || values.Length == 0)
            {
                return null;
            }

            _recordNumber += 1;

            CsvFormat format = overrideFormat ?? Format;
            CsvHeader? header = overrideFormat == null ? Header : Header?.WithFormat(format);
            values ??= Array.Empty<string>();

            return new CsvRecord(header, values, format);
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> over the records of this reader csv,
        /// this enumerable will read the records using this reader, so when the iteration
        /// end the reader will be at the end of the file.
        /// </summary>
        /// <param name="overrideFormat">Overrides the format used for this reader to read the records.</param>
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public RecordsEnumerator ReadAll(CsvFormat? overrideFormat = null)
        {
            ThrowIfDisposed();
            return new RecordsEnumerator(this, overrideFormat?? Format);
        }

        /// <summary>
        /// Gets an <see cref="IAsyncEnumerable{T}"/> over the records of this reader csv,
        /// this enumerable will read the records using this reader, so when the iteration
        /// end the reader will be at the end of the file.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for cancelling this operation</param>
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public RecordsAsyncEnumerator ReadAllAsync(CsvFormat? overrideFormat = null, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            return new RecordsAsyncEnumerator(this, overrideFormat?? Format, cancellationToken);
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
            if (_reader == null)
            {
                return false;
            }

            Stream stream = _reader!.BaseStream!;

            if (_reader.TryReset())
            {
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
            if (_reader == null)
            {
                throw new ObjectDisposedException($"{nameof(CsvReader)} has been disposed");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_reader != null)
            {
                if (disposing)
                {
                    _reader.Dispose();
                }
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

        ~CsvReader()
        {
            Dispose(true);
        }
    }
}
