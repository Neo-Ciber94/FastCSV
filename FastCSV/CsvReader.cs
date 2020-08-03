using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Provides a mechanism for read to csv documents.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class CsvReader : IDisposable
    {
        private StreamReader? _reader;
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
        public CsvReader(StreamReader reader) : this(reader, CsvFormat.Default, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        public CsvReader(StreamReader reader, bool hasHeader) : this(reader, CsvFormat.Default, hasHeader) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        public CsvReader(StreamReader reader, CsvFormat format) : this(reader, format, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        public CsvReader(StreamReader reader, CsvFormat format, bool hasHeader)
        {
            _reader = reader;
            Format = format;

            if (hasHeader)
            {
                List<string>? values = CsvUtility.ReadRecord(_reader!, Format);
                if (values != null && values.Count > 0)
                {
                    Header = new CsvHeader(values, format);
                    _recordNumber += 1;
                }
            }
        }

        /// <summary>
        /// Froms the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static CsvReader FromStream(Stream stream)
        {
            return FromStream(stream, CsvFormat.Default, true);
        }

        /// <summary>
        /// Froms the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        /// <returns></returns>
        public static CsvReader FromStream(Stream stream, bool hasHeader)
        {
            return FromStream(stream, CsvFormat.Default, hasHeader);
        }

        /// <summary>
        /// Froms the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static CsvReader FromStream(Stream stream, CsvFormat format)
        {
            return FromStream(stream, format, true);
        }

        /// <summary>
        /// Froms the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="format">The format.</param>
        /// <param name="hasHeader">if set to <c>true</c> the first record will be considered the header.</param>
        /// <returns></returns>
        public static CsvReader FromStream(Stream stream, CsvFormat format, bool hasHeader)
        {
            StreamReader reader = new StreamReader(stream);
            return new CsvReader(reader, format, hasHeader);
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
        /// Gets a value indicating whether this <see cref="CsvReader"/> had read all the records.
        /// </summary>
        /// <value>
        ///   <c>true</c> if done; otherwise, <c>false</c>.
        /// </value>
        public bool IsDone => _reader?.EndOfStream ?? false;

        /// <summary>
        /// Reads the next record.
        /// </summary>
        /// <returns>The next record or null is there is not more records</returns>
        public CsvRecord? Read()
        {
            ThrowIfDisposed();

            List<string>? values = CsvUtility.ReadRecord(_reader!, Format);

            if (Format.IgnoreWhitespace && (values == null || values.Count == 0))
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
        /// <returns>The next record or null is there is not more records</returns>
        public async Task<CsvRecord?> ReadAsync()
        {
            return await Task.Run(() => Read());
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> over the records of this reader csv,
        /// this enumerable will read the records using this reader, so when the iteration
        /// end the reader will be at the end of the file.
        /// </summary>
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public Records ReadAll()
        {
            ThrowIfDisposed();
            return new Records(this);
        }

        /// <summary>
        /// Gets an <see cref="IAsyncEnumerable{T}"/> over the records of this reader csv,
        /// this enumerable will read the records using this reader, so when the iteration
        /// end the reader will be at the end of the file.
        /// </summary>
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public RecordsAsync ReadAllAsync()
        {
            ThrowIfDisposed();
            return new RecordsAsync(this);
        }

        /// <summary>
        /// Reads the next record as a value of type T.
        /// </summary>
        /// <typeparam name="T">Type to cast the record to.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>An optional with the value or none is there is no more records to read.</returns>
        public Optional<T> ReadAs<T>(ParserDelegate? parser = null)
        {
            Dictionary<string, string>? data = Read()?.ToDictionary();

            if(data == null)
            {
                return Optional.None<T>();
            }

            var result = CsvUtility.CreateInstance<T>(data, parser);
            return Optional.Some(result);
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> over the records of this reader csv and parser them to the type T,
        /// this enumerable will read the records using this reader, so when the iteration
        /// end the reader will be at the end of the file.
        /// </summary>
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public IEnumerable<T> ReadAllAs<T>(ParserDelegate? parser = null)
        {
            return ReadAll().Select(record =>
            {
                Dictionary<string, string> data = record.ToDictionary()!;
                return CsvUtility.CreateInstance<T>(data, parser);
            });
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

            var stream = _reader!.BaseStream;

            if (stream.CanSeek)
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
            if(_reader == null)
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

                _reader = null;
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
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~CsvReader()
        {
            Dispose(true);
        }

        /// <summary>
        /// Represents an iterator over the records of a csv.
        /// <para>
        /// The iterator consume each value after advance and don't creates a new enumerator if <see cref="Records.GetEnumerator()"/> is called
        /// just returns itself. To reset the enumerator use <see cref="Records.Reset()"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEnumerator{FastCSV.CsvRecord}" />
        /// <seealso cref="System.Collections.Generic.IEnumerable{FastCSV.CsvRecord}" />
        public struct Records : IEnumerator<CsvRecord>, IEnumerable<CsvRecord>
        {
            private readonly CsvReader _reader;
            private CsvRecord? _record;

            internal Records(CsvReader reader)
            {
                _reader = reader;
                _record = null;
            }

            public CsvRecord Current => _record!;

            object? IEnumerator.Current => _record;

            public bool IsDone => _reader.IsDone;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_reader.IsDone)
                {
                    return false;
                }

                if ((_record = _reader.Read()) == null)
                {
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                _reader.Reset();
                //_record = null;
            }

            public Records GetEnumerator() => this;

            IEnumerator<CsvRecord> IEnumerable<CsvRecord>.GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => this;
        }

        /// <summary>
        /// Represents an asynchronous iterator over the records of a csv.
        /// <para>
        /// The iterator consume each value after advance and don't creates a new enumerator if <see cref="Records.GetAsyncEnumerator()"/> is called
        /// just returns itself. To reset the enumerator use <see cref="Records.Reset()"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IAsyncEnumerator{FastCSV.CsvRecord}" />
        /// <seealso cref="System.Collections.Generic.IAsyncEnumerable{FastCSV.CsvRecord}" />
        public struct RecordsAsync : IAsyncEnumerator<CsvRecord>, IAsyncEnumerable<CsvRecord>
        {
            private readonly CsvReader _reader;
            private CsvRecord? _record;

            internal RecordsAsync(CsvReader reader)
            {
                _reader = reader;
                _record = null;
            }

            public CsvRecord Current => _record!;

            public bool IsDone => _reader.IsDone;

            public ValueTask<bool> MoveNextAsync()
            {
                if (_reader.IsDone)
                {
                    return new ValueTask<bool>(false);
                }

                if ((_record = _reader.Read()) == null)
                {
                    return new ValueTask<bool>(false);
                }

                return new ValueTask<bool>(true);
            }

            public ValueTask DisposeAsync() => default;

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _reader.Reset();
                //_record = null;
            }

            public RecordsAsync GetEnumerator(CancellationToken cancellationToken = default) => this;

            public IAsyncEnumerator<CsvRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default) => this;
        }
    }
}
