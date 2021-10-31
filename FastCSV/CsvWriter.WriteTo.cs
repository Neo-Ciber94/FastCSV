using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FastCSV
{
    public partial class CsvWriter
    {
        /// <summary>
        /// Writes the given values into the specified file as csv.
        /// </summary>
        /// <typeparam name="T">Type of the data to write.</typeparam>
        /// <param name="values">The values to write.</param>
        /// <param name="path">The destination path.</param>
        /// <param name="format">The format used to write the data.</param>
        /// <param name="flexible">If set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <param name="append">If <c>true</c> the data will be written at the end of the file.</param>
        public static void WriteValuesToFile<T>(IEnumerable<T> values, string path, CsvFormat? format = null, bool flexible = false, bool append = false)
        {
            format ??= CsvFormat.Default;
            CsvHeader header = CsvHeader.FromType<T>(format);
            IEnumerable<CsvRecord> records = values.Select(e => CsvRecord.From(e, format));
            WriteToFile(records, header, path, flexible, append);
        }

        /// <summary>
        /// Writes the given data into the specified file as csv.
        /// </summary>
        /// <param name="records">The records to write.</param>
        /// <param name="header">The header.</param>
        /// <param name="path">The destination path.</param>
        /// <param name="flexible">If set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <param name="append">If <c>true</c> the data will be written at the end of the file.</param>
        public static void WriteToFile(IEnumerable<CsvRecord> records, CsvHeader? header, string path, bool flexible = false, bool append = false)
        {
            FileMode fileMode = append ? FileMode.OpenOrCreate | FileMode.Append : FileMode.OpenOrCreate;
            FileStream fileStream = new(path, fileMode);
            WriteToStream(records, header, fileStream, flexible, leaveOpen: false);
        }

        /// <summary>
        /// Writes the given values into the specified file as csv.
        /// </summary>
        /// <typeparam name="T">Type of the data to write.</typeparam>
        /// <param name="values">The values to write.</param>
        /// <param name="path">The destination path.</param>
        /// <param name="format">The format used to write the data.</param>
        /// <param name="flexible">If set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <param name="append">If <c>true</c> the data will be written at the end of the file.</param>
        /// <param name="cancellationToken">The token to cancel this operation.</param>
        public static async Task WriteValuesToFileAsync<T>(IEnumerable<T> values, string path, CsvFormat? format = null, bool flexible = false, bool append = false, CancellationToken cancellationToken = default)
        {
            format ??= CsvFormat.Default;
            CsvHeader header = CsvHeader.FromType<T>(format);
            IEnumerable<CsvRecord> records = values.Select(e => CsvRecord.From(e, format));
            await WriteToFileAsync(records, header, path, flexible, append, cancellationToken);
        }

        /// <summary>
        /// Writes the given data into the specified file as csv asynchronously.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="records">The records to write.</param>
        /// <param name="path">The destination path.</param>
        /// <param name="flexible">If set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <param name="append">If <c>true</c> the data will be written at the end of the file.</param>
        /// <param name="cancellationToken">The token to cancel this operation.</param>
        public static async Task WriteToFileAsync(IEnumerable<CsvRecord> records, CsvHeader? header, string path, bool flexible = false, bool append = false, CancellationToken cancellationToken = default)
        {
            FileMode fileMode = append ? FileMode.OpenOrCreate | FileMode.Append : FileMode.OpenOrCreate;
            FileStream fileStream = new(path, fileMode);
            await WriteToStreamAsync(records, header, fileStream, flexible, leaveOpen: false, cancellationToken);
        }

        /// <summary>
        /// Writes the given values into the specified <see cref="Stream"/> as a csv, the stream will be closed after use.
        /// </summary>
        /// <typeparam name="T">Type of the values.</typeparam>
        /// <param name="values">The values to write.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="format">The format used to write the records.</param>
        /// <param name="leaveOpen">Whether if close the stream after write the data, default is true.</param>
        public static void WriteValuesToStream<T>(IEnumerable<T> values, Stream destination, CsvFormat? format = null, bool leaveOpen = true)
        {
            format ??= CsvFormat.Default;
            CsvHeader header = CsvHeader.FromType<T>(format);
            IEnumerable<CsvRecord> records = values.Select(e => CsvRecord.From(e, format));
            WriteToStream(records, header, destination, leaveOpen);
        }

        /// <summary>
        /// Writes the given data into the specified <see cref="Stream"/> as a csv, the stream will be closed after use.
        /// </summary>
        /// <param name="records">The records of the csv.</param>
        /// <param name="header">The header.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <param name="leaveOpen">Whether if close the stream after write the data, default is true.</param>
        public static void WriteToStream(IEnumerable<CsvRecord> records, CsvHeader? header, Stream destination, bool flexible = false, bool leaveOpen = true)
        {
            int? recordLength = header?.Length;
            CsvFormat? format = header?.Format;

            if (format == null)
            {
                CsvRecord? firstRecord = records.FirstOrDefault();
                recordLength = firstRecord?.Length;
                format = firstRecord?.Format ?? CsvFormat.Default;
            }

            using CsvWriter writer = new CsvWriter(destination, format, flexible, leaveOpen);

            if (header != null)
            {
                writer.WriteAll(header);
            }

            foreach (var record in records)
            {
                if (header != null && header != record.Header)
                {
                    throw new ArgumentException($"Header mismatch, expected {header} but was {record.Header}");
                }

                if (format != record.Format)
                {
                    throw new ArgumentException("Invalid csv format in record: " + record);
                }

                if (!flexible && recordLength != record.Length)
                {
                    throw new InvalidOperationException($"Invalid record length expected {recordLength} but was {record.Length}");
                }

                writer.WriteAll(record);
            }
        }

        /// <summary>
        /// Writes the given values into the specified <see cref="Stream"/> asychronously as a csv, the stream will be closed after use.
        /// </summary>
        /// <typeparam name="T">Type of the values.</typeparam>
        /// <param name="values">The values to write.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="format">The format used to write the records.</param>
        /// <param name="leaveOpen">Whether if close the stream after write the data, default is true.</param>
        /// <param name="cancellationToken">The token to cancel this operation.</param>
        public static async Task WriteValuesToStreamAsync<T>(IEnumerable<T> values, Stream destination, CsvFormat? format = null, bool leaveOpen = true, CancellationToken cancellationToken = default)
        {
            format ??= CsvFormat.Default;
            CsvHeader header = CsvHeader.FromType<T>(format);
            IEnumerable<CsvRecord> records = values.Select(e => CsvRecord.From(e, format));
            await WriteToStreamAsync(records, header, destination, false, leaveOpen, cancellationToken);
        }

        /// <summary>
        /// Writes the given data into the specified <see cref="Stream"/> asyncrhonously as a csv, the stream will be closed after use.
        /// </summary>
        /// <param name="records">The records of the csv.</param>
        /// <param name="header">The header.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <param name="leaveOpen">Whether if close the stream after write the data, default is true.</param>
        /// <param name="cancellationToken">The token to cancel this operation.</param>
        public static async Task WriteToStreamAsync(IEnumerable<CsvRecord> records, CsvHeader? header, Stream destination, bool flexible = false, bool leaveOpen = true, CancellationToken cancellationToken = default)
        {
            int? recordLength = header?.Length;
            CsvFormat? format = header?.Format;

            if (format == null)
            {
                CsvRecord? firstRecord = records.FirstOrDefault();
                recordLength = firstRecord?.Length;
                format = firstRecord?.Format ?? CsvFormat.Default;
            }

            using CsvWriter writer = new CsvWriter(destination, format, flexible, leaveOpen);

            if (header != null)
            {
                await writer.WriteAllAsync(header, cancellationToken);
            }

            foreach (CsvRecord record in records)
            {
                if (header != null && header != record.Header)
                {
                    throw new ArgumentException($"Header mismatch, expected {header} but was {record.Header}");
                }

                if (format != record.Format)
                {
                    throw new ArgumentException("Invalid csv format in record: " + record);
                }

                if (!flexible && recordLength != record.Length)
                {
                    throw new InvalidOperationException($"Invalid record length expected {recordLength} but was {record.Length}");
                }

                await writer.WriteAllAsync(record, cancellationToken);
            }
        }
    }
}
