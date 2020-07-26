using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastCSV.Utils
{
    public static class CsvUtility
    {
        public static List<string>? ReadRecord(StreamReader reader, CsvFormat format)
        {
            if (reader.EndOfStream)
            {
                return null;
            }

            List<string> records = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            char delimiter = format.Delimiter;
            char quote = format.Quote;
            QuoteStyle style = format.Style;
            bool hasQuote = false;

            // If the record don't contains multi-line values, this outer loop will only run once
            while (true)
            {
                string? line = reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                // Ignore empty entries if the format don't allow whitespaces
                if (format.IgnoreWhitespace && line.IsBlank())
                {
                    continue;
                }

                // Convert the CharEnumerator into an IIterator
                // which allow to inspect the next elements
                IIterator<char> enumerator = line.GetEnumerator().AsIterator();

                while (enumerator.MoveNext())
                {
                    char nextChar = enumerator.Current;

                    // We ignore any CR (carrier return) or LF (line-break)
                    if (!hasQuote && (nextChar == '\r' || nextChar == '\n'))
                    {
                        continue;
                    }

                    // We ignore whitespaces if needed
                    if(!hasQuote && format.IgnoreWhitespace && char.IsWhiteSpace(nextChar))
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
                            records.Add(stringBuilder.ToString());
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

                                //stringBuilder.Append(nextChar);
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

                            // stringBuilder.Append(quote);
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

            return records;
        }

        public static void WriteRecord(StreamWriter writer, IEnumerable<string> values, CsvFormat format)
        {
            string record = ToCsvString(values, format);
            writer.WriteLine(record);
            writer.Flush();
        }

        public static string ToCsvString(IEnumerable<string> values, CsvFormat format)
        {
            if (values.Count() == 0)
            {
                return string.Empty;
            }

            // Helper local function to add quotes
            string AddQuote(string s)
            {
                return string.Concat(format.Quote, s, format.Quote);
            }

            StringBuilder stringBuilder = new StringBuilder();
            IEnumerator<string> enumerator = values.GetEnumerator();
            QuoteStyle style = format.Style;

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    string field = enumerator.Current;

                    if (format.IgnoreWhitespace)
                    {
                        field = field.Trim();
                    }

                    switch (style)
                    {
                        case QuoteStyle.Always:
                            if (!field.EnclosedWith(format.Quote))
                            {
                                field = AddQuote(field);
                            }
                            break;
                        case QuoteStyle.Never:
                            // Remove quotes and line breaks if the style don't allow quotes
                            field = field.Replace("\"", string.Empty)
                                         .Replace("\r", string.Empty)
                                         .Replace("\n", string.Empty);
                            break;
                        case QuoteStyle.WhenNeeded:
                            if (field.Contains(format.Quote) || field.Contains("\n"))
                            {
                                field = AddQuote(field);
                            }
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

        public static List<string> GetValues<T>(T value)
        {
            if(value == null)
            {
                throw new ArgumentException(nameof(value));
            }

            List<string> result = new List<string>();
            Type type = value!.GetType();

            if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(DateTime) || type == typeof(TimeSpan))
            {
                result.Add(value.ToString()!);
            }
            else if (type.IsAssignableFrom(typeof(IEnumerable)))
            {
                foreach (object? e in (IEnumerable)value)
                {
                    result.Add(e?.ToString() ?? "");
                }
            }
            else
            {
                var fields = type.GetFields()
                    .Where(f => f.IsPublic && !f.IsStatic);

                var props = type.GetProperties()
                    .Where(p => p.CanRead);

                if (!fields.Any() && !props.Any())
                {
                    throw new ArgumentException($"Not public fields or properties available for type {typeof(T)}");
                }

                foreach (var f in fields)
                {
                    object? e = f.GetValue(value);
                    result.Add(e?.ToString() ?? "");
                }

                foreach (var p in props)
                {
                    object? e = p.GetValue(value);
                    result.Add(e?.ToString() ?? "");
                }
            }

            return result;
        }

        public static List<string> GetHeader<T>()
        {
            List<string> result = new List<string>();
            Type type = typeof(T);

            if (type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan)
                || type.IsAssignableFrom(typeof(IEnumerable)))
            {
                result.Add(type.Name);
            }
            else
            {
                var fields = type.GetFields()
                    .Where(f => f.IsPublic && !f.IsStatic);

                var props = type.GetProperties()
                    .Where(p => p.CanRead);

                if (!fields.Any() && !props.Any())
                {
                    throw new ArgumentException($"Not public fields or properties available for type {typeof(T)}");
                }

                foreach (var f in fields)
                {
                    result.Add(f.Name);
                }

                foreach (var p in props)
                {
                    result.Add(p.Name);
                }
            }

            return result;
        }

        public static Dictionary<string, object?> ToDictionary<T>(T value)
        {
            if (value == null)
            {
                throw new ArgumentException(nameof(value));
            }

            Dictionary<string, object?> result = new Dictionary<string, object?>();
            Type type = value!.GetType();

            if (type.IsAssignableFrom(typeof(IEnumerable)))
            {
                throw new ArgumentException("Cannot convert an enumerable to dictionary. CsvUtility.ToDictionary should be call for each value");
            }

            if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(DateTime) || type == typeof(TimeSpan))
            {
                result.Add(type.Name, value);
            }
            else
            {
                var fields = type.GetFields()
                    .Where(f => f.IsPublic && !f.IsStatic);

                var props = type.GetProperties()
                    .Where(p => p.CanRead);

                if (!fields.Any() && !props.Any())
                {
                    throw new ArgumentException($"Not public fields or properties available for type {typeof(T)}");
                }

                foreach (var f in fields)
                {
                    object? e = f.GetValue(value);
                    result.Add(f.Name, e);
                }

                foreach (var p in props)
                {
                    object? e = p.GetValue(value);
                    result.Add(p.Name, e);
                }
            }

            return result;
        }
    }
}
