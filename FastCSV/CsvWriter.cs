using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Provides a mechanism for write to csv documents.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class CsvWriter : IDisposable
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
        public CsvWriter(StreamWriter writer) : this(writer, CsvFormat.Default, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        public CsvWriter(StreamWriter writer, bool flexible) : this(writer, CsvFormat.Default, flexible) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        public CsvWriter(StreamWriter writer, CsvFormat format) : this(writer, format, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        public CsvWriter(StreamWriter writer, CsvFormat format, bool flexible)
        {
            _writer = writer;
            IsFlexible = flexible;
            Format = format;
        }

        /// <summary>
        /// Creates a <see cref="CsvWriter"/> from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static CsvWriter FromStream(Stream stream)
        {
            return FromStream(stream, CsvFormat.Default, false);
        }

        /// <summary>
        /// Creates a <see cref="CsvWriter"/> from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <returns></returns>
        public static CsvWriter FromStream(Stream stream, bool flexible)
        {
            return FromStream(stream, CsvFormat.Default, flexible);
        }

        /// <summary>
        /// Creates a <see cref="CsvWriter"/> from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static CsvWriter FromStream(Stream stream, CsvFormat format)
        {
            return FromStream(stream, format, false);
        }

        /// <summary>
        /// Creates a <see cref="CsvWriter"/> from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <returns></returns>
        public static CsvWriter FromStream(Stream stream, CsvFormat format, bool flexible)
        {
            StreamWriter writer = new StreamWriter(stream);
            return new CsvWriter(writer, format, flexible);
        }

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
        public void Write(params object[] values)
        {
            var array = values.Select(e => e?.ToString() ?? string.Empty);
            WriteAll(array);
        }

        /// <summary>
        /// Writes all the values of the specified enumerable.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <exception cref="ArgumentException">If the writer is flexible and attempt to write more fields than the previous one.</exception>
        public void WriteAll(IEnumerable<string> values)
        {
            ThrowIfDispose();

            int length = values.Count();

            // Check if the number of values to write
            // is equals to the last written
            if (!IsFlexible)
            {
                if (_fieldsWritten == 0)
                {
                    _fieldsWritten = length;
                }
                
                if (_fieldsWritten > 0 && _fieldsWritten != length)
                {
                    throw new ArgumentException($"Expected to write {_fieldsWritten} values but {length} was get");
                }
            }

            CsvUtility.WriteRecord(_writer!, values, Format);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="value">The value to write.</param>
        public void WriteWith<T>(T value)
        {
            List<string> values = CsvUtility.GetValues(value);
            WriteAll(values);
        }

        /// <summary>
        /// Writes the specified values asyncronously.
        /// </summary>
        /// <param name="values">The values.</param>
        public async Task WriteAsync(params object[] values)
        {
            await Task.Run(() => Write(values));
        }

        /// <summary>
        /// Writes all the values of the specified enumerable asyncronously.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <exception cref="ArgumentException">If the writer is flexible and attempt to write more fields than the previous one.</exception>
        public async Task WriteAllAsync(IEnumerable<string> values)
        {
            await Task.Run(() => WriteAll(values));
        }

        /// <summary>
        /// Writes the specified value asyncronously.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="value">The value to write.</param>
        public async Task WriteWithAsync<T>(T value)
        {
            await Task.Run(() => WriteWith(value));
        }

        protected void ThrowIfDispose()
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
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
