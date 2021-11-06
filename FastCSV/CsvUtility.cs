using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastCSV.Collections;
using FastCSV.Extensions;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Utility class for work with CSV and provides low level operations.
    /// </summary>
    public static class CsvUtility
    {
        /// <summary>
        /// Parses and reads the next csv record using the specified <see cref="TextReader"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        /// <returns>A list with the fields of the record</returns>
        /// <exception cref="FastCSV.CsvFormatException">If a quote is not closed.</exception>
        public static string[]? ParseNextRecord(TextReader reader, CsvFormat format)
        {
            if ((reader is StreamReader sr && sr.EndOfStream) || reader.Peek() == -1)
            {
                return null;
            }

            StringBuilder stringBuilder = StringBuilderCache.Acquire(512);
            ArrayBuilder<string> records = new ArrayBuilder<string>(16);

            string delimiter = format.Delimiter;
            string quote = format.Quote;
            QuoteStyle style = format.Style;
            bool hasQuote = false;

            // Position used for track current line and offset to provide information in case of errors.
            Position currentPosition = Position.Zero;
            Position quotePosition = Position.Zero;

            try
            {
                // If the record don't contains multi-line values, this outer loop will only run once
                while (true)
                {
                    string? line = reader.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    currentPosition = currentPosition
                        .AddLine(1)
                        .WithOffset(0);

                    // Ignore empty entries if the format don't allow whitespaces
                    if (format.IgnoreWhitespace && string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    // An iterator over the chars of the line
                    TextParser parser = new(line);

                    while (true)
                    {
                        Optional<char> next = parser.Peek();

                        if (!next.HasValue)
                        {
                            break;
                        }

                        char nextChar = next.Value;
                        currentPosition = currentPosition.AddOffset(1);

                        // We ignore any CR (carrier return) or LF (line-break)
                        if (!hasQuote && (nextChar == '\r' || nextChar == '\n'))
                        {
                            continue;
                        }

                        if (parser.CanConsume(delimiter))
                        {
                            parser.Consume(delimiter);

                            if (hasQuote)
                            {
                                stringBuilder.Append(nextChar);
                            }
                            else
                            {
                                WriteFieldToRecord(ref records, stringBuilder, format);
                                stringBuilder.Clear();
                            }
                        }
                        else if (parser.CanConsume(quote))
                        {
                            if (stringBuilder.Length > 0)
                            {
                                if (!hasQuote && parser.HasNext() && !parser.Slice(quote.Length).TrimStart().CanConsume(delimiter))
                                {
                                    throw new CsvFormatException($"Expected a quote escape: {currentPosition}");
                                }
                            }

                            if (hasQuote)
                            {
                                // If the next char is a quote, the current is an escape so ignore it and append the next char
                                // Example: ""red"",other => "red",other
                                if (parser.Slice(quote.Length).CanConsume(quote) && parser.Consume(quote) > 0)
                                {
                                    currentPosition = currentPosition.AddOffset(1);

                                    if (style != QuoteStyle.Never)
                                    {
                                        stringBuilder.Append(parser.Peek().Value);
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
                                            if (!parser.HasNext() || !parser.TrimStart().CanConsume(delimiter))
                                            {
                                                stringBuilder.Append(quote);
                                            }
                                            break;
                                    }

                                    // Quote is close if there is no more elements or the next charater is a delimiter:
                                    // "Field", "Field"
                                    //       ^--end   ^--end                 
                                    TextParser slice = parser.Slice(quote.Length).TrimStart();

                                    if (!slice.HasNext() || slice.CanConsume(delimiter))
                                    {
                                        hasQuote = false;
                                    }
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
                                        stringBuilder.Append(quote);
                                        break;
                                }

                                quotePosition = currentPosition;
                                hasQuote = true;
                            }

                            parser.Consume(quote);
                        }
                        else
                        {
                            stringBuilder.Append(nextChar);
                            parser.Next();
                        }
                    }

                    if (hasQuote)
                    {
                        if (!format.IgnoreNewLine)
                        {
                            stringBuilder.AppendLine();
                        }
                    }
                    else
                    {
                        // Add the last record value
                        WriteFieldToRecord(ref records, stringBuilder, format);
                    }

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

                return records.ToArray();
            }
            finally
            {
                StringBuilderCache.Release(ref stringBuilder!);
                records.Dispose();
            }

            static void WriteFieldToRecord(ref ArrayBuilder<string> builder, StringBuilder stringBuilder, CsvFormat format)
            {
                if (format.IgnoreWhitespace)
                {
                    stringBuilder.Trim();
                }

                string delimiter = format.Delimiter;
                string quote = format.Quote;
                QuoteStyle style = format.Style;

                switch (style)
                {
                    case QuoteStyle.Always:
                        if (!stringBuilder.StartsWith(quote) && !stringBuilder.EndsWith(quote))
                        {
                            stringBuilder.PadLeft(quote);
                            stringBuilder.PadRight(quote);
                        }
                        break;
                    case QuoteStyle.Never:
                        if (stringBuilder.StartsWith(quote) && stringBuilder.EndsWith(quote))
                        {
                            stringBuilder.TrimStartOnce(quote);
                            stringBuilder.TrimEndOnce(quote);
                        }
                        break;
                    case QuoteStyle.WhenNeeded:
                        break;
                }

                if (stringBuilder.Contains(quote) || stringBuilder.Contains(delimiter) || stringBuilder.Contains('\n'))
                {
                    if (!stringBuilder.StartsWithIgnoreWhiteSpace(quote) || !stringBuilder.EndsWithIgnoreWhiteSpace(quote))
                    {
                        throw new CsvFormatException($"Fields containing a delimiter, newline or quote should be enclosed with double quote: {stringBuilder}");
                    }
                }

                builder.Add(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// Reads the next csv record using the specified <see cref="StreamReader"/> asynchronously.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="format">The format.</param>
        /// <returns>A list with the fields of the record</returns>
        /// <exception cref="FastCSV.CsvFormatException">If a quote is not closed.</exception>
        public static async Task<string[]?> ParseNextRecordAsync(StreamReader reader, CsvFormat format, CancellationToken cancellationToken = default)
        {
            if ((reader is StreamReader sr && sr.EndOfStream) || reader.Peek() == -1)
            {
                return null;
            }

            StringBuilder stringBuilder = StringBuilderCache.Acquire(512);
            using ArrayBuilder<string> records = new ArrayBuilder<string>(16);

            string delimiter = format.Delimiter;
            string quote = format.Quote;
            QuoteStyle style = format.Style;
            bool hasQuote = false;

            // Position used for track current line and offset to provide information in case of errors.
            Position currentPosition = Position.Zero;
            Position quotePosition = Position.Zero;

            // If the record don't contains multi-line values, this outer loop will only run once
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string? line = await reader.ReadLineAsync();

                if (line == null)
                {
                    break;
                }

                currentPosition = currentPosition
                    .AddLine(1)
                    .WithOffset(0);

                // Ignore empty entries if the format don't allow whitespaces
                if (format.IgnoreWhitespace && string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // An iterator over the chars of the line
                TextParser parser = new(line);

                while (true)
                {
                    Optional<char> next = parser.Peek();

                    if (!next.HasValue)
                    {
                        break;
                    }

                    char nextChar = next.Value;
                    currentPosition = currentPosition.AddOffset(1);

                    // We ignore any CR (carrier return) or LF (line-break)
                    if (!hasQuote && (nextChar == '\r' || nextChar == '\n'))
                    {
                        continue;
                    }

                    if (parser.CanConsume(delimiter))
                    {
                        parser.Consume(delimiter);

                        if (hasQuote)
                        {
                            stringBuilder.Append(nextChar);
                        }
                        else
                        {
                            if (format.IgnoreWhitespace)
                            {
                                stringBuilder.Trim();
                            }

                            records.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                        }
                    }
                    else if (parser.CanConsume(quote))
                    {
                        parser.Consume(quote);

                        if (hasQuote)
                        {
                            // If the next char is a quote, the current is an escape so ignore it and append the next char:
                            // Example: ""red"",other => "red",other
                            if (parser[1..].CanConsume(quote) && parser.Consume(quote) > 0)
                            {
                                currentPosition = currentPosition.AddOffset(1);

                                if (style != QuoteStyle.Never)
                                {
                                    stringBuilder.Append(parser.Peek().Value);
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
                                        if (!parser.HasNext() || !parser.CanConsume(delimiter))
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
                        parser.Next();
                    }
                }

                // Add the last record value
                if (format.IgnoreWhitespace)
                {
                    stringBuilder.Trim();
                }

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

            StringBuilderCache.Release(ref stringBuilder!);
            return records.ToArray();
        }

        /// <summary>
        /// Writes a csv record using the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="values">The values to write.</param>
        /// <param name="format">The format used to write the record.</param>
        public static void WriteRecord(TextWriter writer, IEnumerable<string> values, CsvFormat format)
        {
            string record = ToCsvString(values, format);
            writer.WriteLine(record);
            writer.Flush();
        }

        /// <summary>
        /// Writes a csv record using the specified <see cref="TextWriter"/> asynchronously.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="values">The values to write.</param>
        /// <param name="format">The format used to write the record.</param>
        public static async Task WriteRecordAsync(TextWriter writer, IEnumerable<string> values, CsvFormat format)
        {
            string record = ToCsvString(values, format);
            await writer.WriteLineAsync(record);
            await writer.FlushAsync();
        }

        /// <summary>
        /// Converts the given <see cref="IEnumerable{T}"/> into a csv string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToCsvString(IEnumerable<string> values, CsvFormat? format = null)
        {
            if (!values.Any())
            {
                return string.Empty;
            }

            format ??= CsvFormat.Default;

            // Helper local function to add quotes
            static string AddQuote(CsvFormat format, string s)
            {
                int length = s.Length + (format.Quote.Length * 2);

                return string.Create(length, (format.Quote, s), static (span, state) =>
                {
                    int quoteLength = state.Quote.Length;
                    int lastQuoteIndex = span.Length - quoteLength;
                    ReadOnlySpan<char> quote = state.Quote;
                    string stringValue = state.s;

                    quote.CopyTo(span);
                    quote.CopyTo(span[lastQuoteIndex..]);
                    stringValue.AsSpan().CopyTo(span[quoteLength..lastQuoteIndex]);
                });
            }

            using var stringBuilder = new ValueStringBuilder(stackalloc char[128]);
            var enumerator = values.GetEnumerator();
            QuoteStyle style = format.Style;

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    string field = enumerator.Current;

                    if (format.IgnoreNewLine)
                    {
                        if (field.Contains('\n'))
                        {
                            field = field
                                .Replace("\r\n", string.Empty)
                                .Replace("\n", string.Empty);
                        }
                    }

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
                                field = AddQuote(format, field);
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
        /// Converts the given <see cref="Array"/> into a csv string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToCsvString(string[] values, CsvFormat? format = null)
        {
            return ToCsvString(values.AsSpan(), format);
        }

        /// <summary>
        /// Converts the given <see cref="ReadOnlySpan{T}"/> into a csv string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToCsvString(ReadOnlySpan<string> values, CsvFormat? format = null)
        {
            if (values.IsEmpty)
            {
                return string.Empty;
            }

            format ??= CsvFormat.Default;

            // Helper local function to add quotes
            static string AddQuote(CsvFormat format, string s)
            {
                int length = s.Length + (format.Quote.Length * 2);

                return string.Create(length, (format.Quote, s), static (span, state) =>
                {
                    int quoteLength = state.Quote.Length;
                    int lastQuoteIndex = span.Length - quoteLength;
                    ReadOnlySpan<char> quote = state.Quote;
                    string stringValue = state.s;

                    quote.CopyTo(span);
                    quote.CopyTo(span[lastQuoteIndex..]);
                    stringValue.AsSpan().CopyTo(span[quoteLength..lastQuoteIndex]);
                });
            }

            using var stringBuilder = new ValueStringBuilder(stackalloc char[128]);
            var enumerator = values.GetEnumerator();
            QuoteStyle style = format.Style;

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    string field = enumerator.Current;

                    if (format.IgnoreNewLine)
                    {
                        if (field.Contains('\n'))
                        {
                            field = field
                                .Replace("\r\n", string.Empty)
                                .Replace("\n", string.Empty);
                        }
                    }

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
                                field = AddQuote(format, field);
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
            return string.Join(Environment.NewLine, values);
        }

        private static string FormatCsvString(string s, CsvFormat format)
        {
            bool encloseWithQuotes = false;
            bool containsQuote = s.Contains(format.Quote);

            if ((s.Contains("\n") || s.Contains(format.Delimiter)) && !s.EnclosedWith(format.Quote))
            {
                encloseWithQuotes = true;
            }

            // Quick path to avoid extra allocations
            if (!containsQuote && !encloseWithQuotes)
            {
                return s;
            }

            StringBuilder stringBuilder = StringBuilderCache.Acquire(s.Length);
            stringBuilder.Append(s);

            if (containsQuote && !s.EnclosedWith(format.Quote))
            {
                encloseWithQuotes = true;

                string doubleQuote = string.Concat(format.Quote, format.Quote);

                // Escape any double quote
                stringBuilder.Replace(format.Quote, doubleQuote);
            }

            if (encloseWithQuotes)
            {
                stringBuilder.PadLeft(format.Quote);
                stringBuilder.PadRight(format.Quote);
            }

            return StringBuilderCache.ToStringAndRelease(ref stringBuilder!);
        }
    }
}