using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Net;
using System.Runtime.CompilerServices;
using FastCSV.Utils;
using FastCSV.Collections;

namespace FastCSV
{
    public partial class CsvDocument
    {
        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified string.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> will allow records of differents lengths.</param>
        /// <returns></returns>
        public static CsvDocument FromCsv(string csv, CsvFormat? format = null, bool flexible = false)
        {
            format ??= CsvFormat.Default;

            if (string.IsNullOrWhiteSpace(csv))
            {
                throw new ArgumentException("CSV is empty");
            }

            using MemoryStream memory = StreamHelper.ToMemoryStream(csv);

            using (CsvReader reader = new CsvReader(new StreamReader(memory), format))
            {
                List<CsvRecord>? records;

                if (flexible)
                {
                    records = reader.ReadAll().ToList();
                }
                else
                {
                    records = new List<CsvRecord>();
                    int headerLength = reader.Header!.Length; // FIXME: What warrantes the header is not null?

                    foreach (CsvRecord r in reader.ReadAll())
                    {
                        int recordLength = r.Length;
                        if (recordLength != headerLength)
                        {
                            throw new InvalidOperationException($"Invalid record length for non-flexible csv, " +
                                $"expected {headerLength} but {recordLength} was get");
                        }

                        records.Add(r);
                    }
                }

                return new CsvDocument(records, reader.Header!, format, flexible);
            }
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from a csv file at the given path.
        /// </summary>
        /// <param name="path">The path of the csv file.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> will allow records of differents lengths.</param>
        /// <returns>A csv document from the file at the given path.</returns>
        public static CsvDocument FromPath(string path, CsvFormat? format = null, bool flexible = false)
        {
            format ??= CsvFormat.Default;

            using (StreamReader streamReader = new StreamReader(path))
            {
                return FromCsv(streamReader.ReadToEnd(), format, flexible);
            }
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified header and records.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="records">The records.</param>
        /// <param name="flexible">if set to <c>true</c> [flexible].</param>
        /// <returns>A document with the specified header and record.</returns>
        public static CsvDocument FromRaw(CsvHeader header, IEnumerable<CsvRecord> records, bool flexible = false)
        {
            List<CsvRecord> result = new List<CsvRecord>();

            foreach (CsvRecord record in records)
            {
                if (record.Header != header)
                {
                    throw new ArgumentException($"Differents header on record: {record}");
                }

                if (!flexible && record.Length != header.Length)
                {
                    throw new ArgumentException($"Invalid length on record, expected {header.Length} elements. Record: {record}");
                }

                result.Add(record);
            }

            return new CsvDocument(result, header, header.Format, flexible);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument{T}"/> from the given csv data.
        /// <para>
        /// The specified type must have public fields and/or setters to initialize the instance and those fields
        /// must be of a valid type like primitives, <see cref="string"/>, <see cref="BigInteger"/>, <see cref="TimeSpan"/>,
        /// <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="Guid"/>, <see cref="Enum"/>, <see cref="IPAddress"/>,
        /// or <see cref="Version"/>.
        /// </para>
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="options">Options used to deserialize the values.</param>
        /// <returns>A csv document from the given data.</returns>
        public static CsvDocument<T> FromCsv<T>(string csv, CsvConverterOptions? options = null)
        {
            List<T> list = new List<T>();
            MemoryStream memory = StreamHelper.ToMemoryStream(csv);

            options ??= CsvConverterOptions.Default;
            CsvFormat format = options.Format;

            using (CsvReader reader = CsvReader.FromStream(memory, format))
            {
                foreach (CsvRecord record in reader.ReadAll())
                {
                    Dictionary<string, SingleOrList<string>>? data = record.ToDictionary()!;
                    T value = CsvConverter.DeserializeFromDictionary<T>(data, options);
                    list.Add(value);
                }
            }

            return new CsvDocument<T>(list, format);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument{T}"/> from a csv file at the given path.
        /// </summary>
        /// <param name="path">The path of the csv file.</param>
        /// <param name="format">The format.</param>
        /// <param name="parser">The parser.</param>
        /// <returns>A csv document from the file at the given path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CsvDocument<T> FromPath<T>(string path, CsvConverterOptions? options = null)
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                return FromCsv<T>(streamReader.ReadToEnd(), options);
            }
        }
    }
}
