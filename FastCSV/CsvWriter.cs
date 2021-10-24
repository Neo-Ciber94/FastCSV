using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastCSV.Collections;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Provides a mechanism for write to csv documents.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class CsvWriter : IDisposable
    {
        private StreamWriter? _writer;
        private int _fieldsWritten = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public CsvWriter(string path) : this(path, CsvFormat.Default, flexible: false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        public CsvWriter(string path, bool flexible) : this(path, CsvFormat.Default, flexible) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="format">The format.</param>
        public CsvWriter(string path, CsvFormat format) : this(path, format, flexible: false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        public CsvWriter(string path, CsvFormat format, bool flexible) : this(new StreamWriter(path, append: true), format, flexible) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        public CsvWriter(StreamWriter writer, CsvFormat? format = null, bool flexible = false)
        {
            _writer = writer;
            IsFlexible = flexible;
            Format = format ?? CsvFormat.Default;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="CsvWriter"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <param name="leaveOpen">Whether if close the given stream after the writer is disposed.</param>
        public CsvWriter(Stream stream, CsvFormat? format = null, bool flexible = false, bool leaveOpen = false) 
            : this(new StreamWriter(stream, leaveOpen: leaveOpen), format, flexible) { }

        /// <summary>
        /// Gets the csv format used for this writer.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public CsvFormat Format { get; }

        /// <summary>
        /// Gets a value indicating whether this writer allows writer records of diferent length.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is flexible; otherwise, <c>false</c>.
        /// </value>
        public bool IsFlexible { get; }

        /// <summary>
        /// Writes the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        public void Write(params object?[] values)
        {
            if (values.Length == 0)
            {
                WriteAll(Array.Empty<string>());
                return;
            }

            using var builder = new ArrayBuilder<string>(values.Length);

            foreach (object? obj in values)
            {
                string s = obj?.ToString() ?? string.Empty;
                builder.Add(s);
            }

            WriteAll(builder.ToArray());
        }

        /// <summary>
        /// Writes all the values of the specified enumerable.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <exception cref="ArgumentException">If the writer is flexible and attempt to write more fields than the previous one.</exception>
        public void WriteAll(IEnumerable<string> values)
        {
            ThrowIfDisposed();
            AssertFieldsToWriteCount(values.Count());
            CsvUtility.WriteRecord(_writer!, values, Format);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="value">The value to write.</param>
        public void WriteValue<T>(T value)
        {
            string[] values = CsvConverter.GetValues(value);
            WriteAll(values);
        }

        /// <summary>
        /// Writes the specified values asyncronously.
        /// </summary>
        /// <param name="values">The values.</param>
        public async Task WriteAsync(params object[] values)
        {
            if (values.Length == 0)
            {
                await WriteAllAsync(Array.Empty<string>());
                return;
            }

            using var builder = new ArrayBuilder<string>(values.Length);

            foreach (object? obj in values)
            {
                string s = obj?.ToString() ?? string.Empty;
                builder.Add(s);
            }

            await WriteAllAsync(builder.ToArray());
        }

        /// <summary>
        /// Writes all the values of the specified enumerable asyncronously.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="cancellationToken">A cancellation token for cancelling this operation.</param>
        /// <exception cref="ArgumentException">If the writer is flexible and attempt to write more fields than the previous one.</exception>
        public async Task WriteAllAsync(IEnumerable<string> values, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            AssertFieldsToWriteCount(values.Count());
            await CsvUtility.WriteRecordAsync(_writer!, values, Format);
        }

        /// <summary>
        /// Writes the specified value asyncronously.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="value">The value to write.</param>
        /// <param name="cancellationToken">A cancellation token for cancelling this operation</param>
        public Task WriteValueAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            WriteValue(value);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears the buffers of this writes and ensures all the data is written.
        /// </summary>
        public void Flush()
        {
            ThrowIfDisposed();
            _writer!.Flush();
        }

        protected void AssertFieldsToWriteCount(int numberOfFieldsToWrite)
        {
            // Check if the number of values to write is equals to the last written
            if (!IsFlexible)
            {
                if (_fieldsWritten == 0)
                {
                    _fieldsWritten = numberOfFieldsToWrite;
                }

                if (_fieldsWritten > 0 && _fieldsWritten != numberOfFieldsToWrite)
                {
                    throw new ArgumentException($"Expected to write {_fieldsWritten} values but {numberOfFieldsToWrite} were written");
                }
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_writer == null)
            {
                throw new ObjectDisposedException($"{nameof(CsvWriter)} has been disposed");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_writer != null)
            {
                if (disposing)
                {
                    _writer.Dispose();
                }

                _writer = null;
            }
        }

        /// <summary>
        /// Releases the resources used for this writer.
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

        ~CsvWriter()
        {
            Dispose(true);
        }
    }
}
