using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public static void WriteToFile<T>(IEnumerable<T> values, string path, CsvFormat? format = null, bool flexible = false, bool append = false)
        {
            format ??= CsvFormat.Default;
            CsvHeader header = CsvHeader.FromType<T>(format);
            IEnumerable<CsvRecord> records = values.Select(e => CsvRecord.From(e, format));
            WriteToFile(header, records, path, flexible, append);
        }

        /// <summary>
        /// Writes the given data into the specified file as csv.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="records">The records to write.</param>
        /// <param name="path">The destination path.</param>
        /// <param name="flexible">If set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        /// <param name="append">If <c>true</c> the data will be written at the end of the file.</param>
        public static void WriteToFile(CsvHeader? header, IEnumerable<CsvRecord> records, string path, bool flexible = false, bool append = false)
        {
            FileMode fileMode = append ? FileMode.OpenOrCreate | FileMode.Append : FileMode.OpenOrCreate;
            FileStream fileStream = new FileStream(path, fileMode);
            WriteToStream(header, records, fileStream, flexible);
        }

        /// <summary>
        /// Writes the given values into the specified <see cref="Stream"/> as a csv, the stream will be closed after use.
        /// </summary>
        /// <typeparam name="T">Type of the values.</typeparam>
        /// <param name="values">The values to write.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="format">The format used to write the records.</param>
        public static void WriteToStream<T>(IEnumerable<T> values, Stream destination, CsvFormat? format = null)
        {
            format ??= CsvFormat.Default;
            CsvHeader header = CsvHeader.FromType<T>(format);
            IEnumerable<CsvRecord> records = values.Select(e => CsvRecord.From(e, format));
            WriteToStream(header, records, destination);
        }

        /// <summary>
        /// Writes the given data into the specified <see cref="Stream"/> as a csv, the stream will be closed after use.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="records">The records of the csv.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="flexible">if set to <c>true</c> the writer will allow records of diferent lenghts.</param>
        public static void WriteToStream(CsvHeader? header, IEnumerable<CsvRecord> records, Stream destination, bool flexible = false)
        {
            int? recordLength = header?.Length;
            CsvFormat? format = header?.Format;

            if (format == null)
            {
                CsvRecord? firstRecord = records.FirstOrDefault();
                recordLength = firstRecord?.Length;
                format = firstRecord?.Format ?? CsvFormat.Default;
            }

            using CsvWriter writer = CsvWriter.FromStream(destination, format, flexible);

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
    }
}
