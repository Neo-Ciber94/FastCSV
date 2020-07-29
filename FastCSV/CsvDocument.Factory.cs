using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using FastCSV.Utils;

namespace FastCSV
{
    public partial class CsvDocument
    {
        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified string.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CsvDocument FromCsv(string csv)
        {
            return FromCsv(csv, CsvFormat.Default, false);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified string.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="flexible">if set to <c>true</c> [flexible].</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CsvDocument FromCsv(string csv, bool flexible)
        {
            return FromCsv(csv, CsvFormat.Default, flexible);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified string.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CsvDocument FromCsv(string csv, CsvFormat format)
        {
            return FromCsv(csv, format, false);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from the specified string.
        /// </summary>
        /// <param name="csv">The CSV.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> will allow records of differents lengths.</param>
        /// <returns></returns>
        public static CsvDocument FromCsv(string csv, CsvFormat format, bool flexible)
        {
            if (csv.IsNullOrBlank())
            {
                throw new ArgumentException("CSV is empty");
            }

            using MemoryStream memory = CsvUtility.ToStream(csv);

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
                    int headerLength = reader.Header!.Length;

                    foreach (var r in reader.ReadAll())
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
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CsvDocument FromPath(string path)
        {
            return FromPath(path, CsvFormat.Default, false);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from a csv file at the given path.
        /// </summary>
        /// <param name="path">The path of the csv file.</param>
        /// <param name="flexible">if set to <c>true</c> [flexible].</param>
        /// <returns>A csv document from the file at the given path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CsvDocument FromPath(string path, bool flexible)
        {
            return FromPath(path, CsvFormat.Default, flexible);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from a csv file at the given path.
        /// </summary>
        /// <param name="path">The path of the csv file.</param>
        /// <param name="format">The format.</param>
        /// <returns>A csv document from the file at the given path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CsvDocument FromPath(string path, CsvFormat format)
        {
            return FromPath(path, format, false);
        }

        /// <summary>
        /// Creates a <see cref="CsvDocument"/> from a csv file at the given path.
        /// </summary>
        /// <param name="path">The path of the csv file.</param>
        /// <param name="format">The format.</param>
        /// <param name="flexible">if set to <c>true</c> will allow records of differents lengths.</param>
        /// <returns>A csv document from the file at the given path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CsvDocument FromPath(string path, CsvFormat format, bool flexible)
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                return FromCsv(streamReader.ReadToEnd(), format, flexible);
            }
        }
    }

    public partial class CsvDocument<T>
    {
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
        /// <param name="format">The format.</param>
        /// <param name="parser">The parser.</param>
        /// <returns>A csv document from the given data.</returns>
        public static CsvDocument<T> FromCsv(string csv, CsvFormat? format = null, ParserDelegate? parser = null)
        {
            List<T> list = new List<T>();
            MemoryStream memory = CsvUtility.ToStream(csv);

            format ??= CsvFormat.Default;

            using (CsvReader reader = CsvReader.FromStream(memory, format))
            {
                foreach (CsvRecord record in reader.ReadAll())
                {
                    Dictionary<string, string> data = record.ToDictionary()!;
                    T value = parser == null ? CsvUtility.CreateInstance<T>(data) : CsvUtility.CreateInstance<T>(data, parser);
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
        public static CsvDocument<T> FromPath(string path, CsvFormat? format = null, ParserDelegate? parser = null)
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                return FromCsv(streamReader.ReadToEnd(), format?? CsvFormat.Default, parser);
            }
        }
    }
}
