using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FastCSV.Utils
{

    /// <summary>
    /// Utility class for work with CSV.
    /// </summary>
    public static class CsvUtility
    {
        /// <summary>
        /// Reads the next csv record using the specified <see cref="StreamReader"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        /// <returns>A list with the fields of the record</returns>
        /// <exception cref="FastCSV.CsvFormatException">If a quote is not closed.</exception>
        public static List<string>? ReadRecord(StreamReader reader, CsvFormat format)
        {
            if (reader.EndOfStream)
            {
                return default;
            }

            using ValueStringBuilder stringBuilder = new ValueStringBuilder(stackalloc char[512]);
            List<string> records = new List<string>();
            char delimiter = format.Delimiter;
            char quote = format.Quote;
            QuoteStyle style = format.Style;
            bool hasQuote = false;

            // Position used for track current line and offset to provide information in case of errors.
            Position currentPosition = Position.Zero;
            Position quotePosition = Position.Zero;

            // If the record don't contains multi-line values, this outer loop will only run once
            while (true)
            {
                string? line = reader.ReadLine();

                currentPosition = currentPosition
                    .AddLine(1)
                    .WithOffset(0);

                if (line == null)
                {
                    break;
                }

                // Ignore empty entries if the format don't allow whitespaces
                if (format.IgnoreWhitespace && string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Convert the CharEnumerator into an IIterator
                // which allow to inspect the next elements
                IIterator<char> enumerator = line.GetEnumerator().AsIterator();

                while (enumerator.MoveNext())
                {
                    char nextChar = enumerator.Current;

                    currentPosition = currentPosition.AddOffset(1);

                    // We ignore any CR (carrier return) or LF (line-break)
                    if (!hasQuote && (nextChar == '\r' || nextChar == '\n'))
                    {
                        continue;
                    }

                    if (nextChar == delimiter)
                    {
                        if (hasQuote)
                        {
                            stringBuilder.Append(nextChar);
                        }
                        else
                        {
                            // Gets the current field and trim the whitespaces if required by the format
                            string field = stringBuilder.ToString();

                            if (format.IgnoreWhitespace)
                            {
                                field = field.Trim();
                            }

                            records.Add(field);
                            stringBuilder.Clear();
                        }
                    }
                    else if (nextChar == quote)
                    {
                        if (hasQuote)
                        {
                            // If the next char is a quote, the current is an escape so ignore it
                            // and append the next char
                            if (enumerator.Peek.Contains(quote) && enumerator.MoveNext())
                            {
                                currentPosition = currentPosition.AddOffset(1);

                                if (style != QuoteStyle.Never)
                                {
                                    stringBuilder.Append(enumerator.Current);
                                }
                            }
                            else
                            {
                                switch (style)
                                {
                                    case QuoteStyle.Always:
                                        stringBuilder.Append(quote);
                                        break;
                                    case QuoteStyle.Never:
                                        break;
                                    case QuoteStyle.WhenNeeded:
                                        if (!enumerator.HasNext() || !enumerator.Peek.Contains(delimiter))
                                        {
                                            stringBuilder.Append(quote);
                                        }
                                        break;
                                }

                                hasQuote = false;
                            }
                        }
                        else
                        {
                            switch (style)
                            {
                                case QuoteStyle.Always:
                                    stringBuilder.Append(quote);
                                    break;
                                case QuoteStyle.Never:
                                    break;
                                case QuoteStyle.WhenNeeded:
                                    if (stringBuilder.Length > 0)
                                    {
                                        stringBuilder.Append(quote);
                                    }
                                    break;
                            }

                            quotePosition = currentPosition;
                            hasQuote = true;
                        }
                    }
                    else
                    {
                        stringBuilder.Append(nextChar);
                    }
                }

                // Add the last record value
                records.Add(stringBuilder.ToString());

                // Exit if we aren't in a quote
                if (!hasQuote)
                {
                    break;
                }
            }

            if (hasQuote)
            {
                throw new CsvFormatException($"Quote wasn't closed. Position: {quotePosition}");
            }

            return records;
        }

        /// <summary>
        /// Writes a csv record using the specified <see cref="StreamWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="values">The values to write.</param>
        /// <param name="format">The format used to write the record.</param>
        public static void WriteRecord(StreamWriter writer, IEnumerable<string> values, CsvFormat format)
        {
            string record = ToCsvString(values, format);
            writer.WriteLine(record);
            writer.Flush();
        }

        /// <summary>
        /// Converts the given <see cref="IEnumerable{T}"/> into a csv string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToCsvString(IEnumerable<string> values, CsvFormat format)
        {
            if (!values.Any())
            {
                return string.Empty;
            }

            // Helper local function to add quotes
            string AddQuote(string s)
            {
                return string.Concat(format.Quote, s, format.Quote);
            }

            using var stringBuilder = new ValueStringBuilder(stackalloc char[128]);
            IEnumerator<string> enumerator = values.GetEnumerator();
            QuoteStyle style = format.Style;

            // Clears the content of the provided StringBuilder
            stringBuilder.Clear();

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    string field = enumerator.Current;

                    if (format.IgnoreWhitespace)
                    {
                        field = field.Trim();
                    }

                    // Ensure the csv field is well formated
                    field = FormatCsvString(field, format);

                    switch (style)
                    {
                        case QuoteStyle.Always:
                            if (!field.EnclosedWith(format.Quote))
                            {
                                field = AddQuote(field);
                            }
                            break;
                        case QuoteStyle.Never:
                            // Remove quotes and line breaks if the style don't allow quotes to avoid format errors
                            field = field.Replace("\"", string.Empty)
                                         .Replace("\r", string.Empty)
                                         .Replace("\n", string.Empty);
                            break;
                        case QuoteStyle.WhenNeeded:
                            // Field is formatted before
                            break;
                    }

                    stringBuilder.Append(field);

                    // If there is more values add a delimiter
                    if (enumerator.MoveNext())
                    {
                        stringBuilder.Append(format.Delimiter);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets a pretty formatted string of the given records.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <returns></returns>
        public static string ToPrettyString(IEnumerable<CsvRecord> records)
        {
            if (!records.Any())
            {
                return string.Empty;
            }

            // Min padding between 2 columns
            const int MinPadding = 5;

            CsvHeader? header = records.FirstOrDefault()?.Header;

            using ValueStringBuilder recordBuilder = new ValueStringBuilder(stackalloc char[64]);
            using ValueStringBuilder resultBuilder = new ValueStringBuilder(stackalloc char[256]);
            int columns = header?.Length ?? records.First().Length;

            // We decide if allocates on the head or stack depending the amount of columns with 100 as thredshold.
            Span<int> columnSizes = columns < 100 ? stackalloc int[columns] : new int[columns];

            // Takes the header lenghts
            if (header != null)
            {
                for (int i = 0; i < columns; i++)
                {
                    columnSizes[i] = header[i].Length;
                }
            }

            // Calculates the max length of each column
            foreach (CsvRecord record in records)
            {
                Debug.Assert(record.Header == header);

                for (int i = 0; i < columns; i++)
                {
                    columnSizes[i] = Math.Max(columnSizes[i], record[i].Length);
                }
            }

            // Writes the header
            if (header != null)
            {
                for (int i = 0; i < columns; i++)
                {
                    string field = header[i];

                    if (i < columns - 1)
                    {
                        field = field.PadRight(columnSizes[i] + MinPadding);
                    }

                    recordBuilder.Append(field);
                }

                resultBuilder.AppendLine(recordBuilder.ToString());
                recordBuilder.Clear();
            }

            // Writes each record and add the min padding
            foreach (CsvRecord record in records)
            {
                for (int i = 0; i < columns; i++)
                {
                    string field = record[i].Trim();

                    if (i < columns - 1)
                    {
                        field = field.PadRight(columnSizes[i] + MinPadding);
                    }

                    recordBuilder.Append(field);
                }

                resultBuilder.AppendLine(recordBuilder.ToString());
                recordBuilder.Clear();
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        /// Combines a list of csv records into a csv separated by newlines.
        /// </summary>
        /// <param name="values">The csv records to combine</param>
        /// <returns>A csv with the records separated by a newline.</returns>
        public static string JoinLines(IEnumerable<string> values)
        {
            return string.Join('\n', values);
        }

        /// <summary>
        /// Gets a <see cref="MemoryStream"/> containing the specified <see cref="string"/> data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A stream containing the specified data.</returns>
        public static MemoryStream ToStream(string data)
        {
            MemoryStream memory = new MemoryStream(data.Length);
            using (var writer = new StreamWriter(memory, leaveOpen: true))
            {
                writer.Write(data);
                writer.Flush();
                memory.Position = 0;
            }

            return memory;
        }

        // FIXME: This method allocates too much
        private static string FormatCsvString(string s, CsvFormat format)
        {
            bool encloseWithQuotes = false;

            if (s.Contains("\n"))
            {
                encloseWithQuotes = true;
            }
            
            if (s.Contains(format.Delimiter))
            {
                encloseWithQuotes = true;
            }

            if (s.Contains(format.Quote))
            {
                encloseWithQuotes = true;

                string doubleQuote = new string(format.Quote, 2);
                s = s.Replace(format.Quote.ToString(), doubleQuote);
            }

            if (encloseWithQuotes)
            {
                s = $"{format.Quote}{s}{format.Quote}";
            }

            return s;
        }
    }
}
