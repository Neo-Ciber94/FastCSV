using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Creates the instance of the specified type using the values of the given <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="data">The data to populate the fields of the new instance.</param>
        /// <param name="parser">A parser used for all the values, for each value the parser will be called
        /// and if fail will attempt to parse using <see cref="CsvUtility.TryParse(string, Type, out object?)"/>.</param>
        /// <returns>A new instance of the specified type.</returns>
        /// <exception cref="InvalidOperationException">If cannot find or parse the fields or properties with the name of keys of the <c>data</c></exception>
        public static T CreateInstance<T>(Dictionary<string, string> data, IEnumerable<IValueParser>? parsers = null)
        {
            Optional<object> result = default;
            Type type = typeof(T);

            foreach (var pair in data)
            {
                FieldInfo? field = GetField(type, pair.Key, BindingFlags.Public | BindingFlags.Instance)?.Field;

                if (field != null)
                {
                    if (!result.HasValue)
                    {
                        result = Optional.Some(FormatterServices.GetUninitializedObject(type));
                    }

                    object? obj = ParseValue(parsers, field.FieldType, field.Name, pair.Key, pair.Value);
                    field.SetValue(result.Value, obj);
                }

                PropertyInfo? prop = GetProperty(type, pair.Key, BindingFlags.Public | BindingFlags.Instance)?.Property;

                if (prop != null)
                {
                    if (!result.HasValue)
                    {
                        result = Optional.Some(FormatterServices.GetUninitializedObject(type));
                    }

                    object? obj = ParseValue(parsers, prop.PropertyType, prop.Name, pair.Key, pair.Value);
                    prop.SetValue(result.Value, obj);
                }

                if (field == null && prop == null)
                {
                    throw new InvalidOperationException("Cannot find a field or property named: " + pair.Key);
                }
            }

            if (!result.HasValue)
            {
                throw new InvalidOperationException($"Unable to initializated an object of type {typeof(T)}");
            }

            return (T)result.Value;

            // Helper method
            static object? ParseValue(IEnumerable<IValueParser>? parsers, Type type, string fieldOrPropertyName, string key, string value)
            {
                if (parsers != null)
                {
                    foreach(IValueParser parser in parsers)
                    {
                        if(parser.TryParse(key, value, out object? obj))
                        {
                            return obj;
                        }
                    }
                }

                if (!TryParse(value, type, out object? result))
                {
                    throw new InvalidOperationException($"Unable to initalize property: {fieldOrPropertyName}");
                }

                return result;
            }
        }

        /// <summary>
        /// Attempts to parse the specified <see cref="string"/> value.
        /// </summary>
        /// <typeparam name="T">The type to parse to.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <param name="result">The result value.</param>
        /// <returns><c>true</c> if the parse success, otherwise <c>false</c></returns>
        public static bool TryParse<T>(string value, [MaybeNullWhen(false)] out T result)
        {
            result = default;
            if (TryParse(value, typeof(T), out var obj))
            {
                result = (T)obj!;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to parse the specified <see cref="string"/> value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="type">The type to parse to.</param>
        /// <param name="result">The result value.</param>
        /// <returns><c>true</c> if the parse success, otherwise <c>false</c></returns>
        public static bool TryParse(string value, Type type, out object? result)
        {
            result = null;

            Type? nullableType = Nullable.GetUnderlyingType(type);

            if(nullableType != null)
            {
                type = nullableType;
            }

            if (type.IsPrimitive)
            {
                if (type == typeof(bool))
                {
                    bool ret = bool.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(char))
                {
                    bool ret = char.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(byte))
                {
                    bool ret = byte.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(sbyte))
                {
                    bool ret = sbyte.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(short))
                {
                    bool ret = short.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(int))
                {
                    bool ret = int.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(long))
                {
                    bool ret = long.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(ushort))
                {
                    bool ret = int.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(uint))
                {
                    bool ret = int.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(ulong))
                {
                    bool ret = long.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(float))
                {
                    bool ret = float.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(double))
                {
                    bool ret = double.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
            }
            else
            {
                // System.Decimal is not a primitive type
                // https://docs.microsoft.com/en-us/dotnet/api/system.type.isprimitive?view=net-5.0#System_Type_IsPrimitive
                if (type == typeof(decimal))
                {
                    bool ret = decimal.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(BigInteger))
                {
                    bool ret = BigInteger.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(TimeSpan))
                {
                    bool ret = TimeSpan.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(DateTime))
                {
                    bool ret = DateTime.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(DateTimeOffset))
                {
                    bool ret = DateTimeOffset.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(Guid))
                {
                    bool ret = Guid.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type.IsEnum)
                {
                    bool ret = Enum.TryParse(type, value, ignoreCase: true, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(IPAddress))
                {
                    bool ret = IPAddress.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(Version))
                {
                    bool ret = Version.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
                else if (type == typeof(string))
                {
                    result = value;
                    return true;
                }
            }

            return false;
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

            CsvHeader? header = records.First().Header;

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

        private static CsvFieldInfo? GetField(Type type, string name, BindingFlags flags)
        {
            var fields = GetFields(type, flags).Where(f => name == f.Field.Name || name == f.Alias);

            // Duplicated fields names are only possible if contains aliases
            if(fields.Count() > 1)
            {
                throw new InvalidOperationException($"Csv fields with duplicated name: ${fields.ElementAt(0).Alias}");
            }

            return fields.SingleOrDefault();
        }

        private static CsvPropertyInfo? GetProperty(Type type, string name, BindingFlags flags)
        {
            var props = GetProperties(type, flags).Where(f => name == f.Property.Name || name == f.Alias);

            // Duplicated properties names are only possible if contains aliases
            if (props.Count() > 1)
            {
                throw new InvalidOperationException($"Csv properties with duplicated name: ${props.ElementAt(0).Alias}");
            }

            return props.SingleOrDefault();
        }

        private static IEnumerable<CsvFieldInfo> GetFields(Type type, BindingFlags flags)
        {
            var result = new List<CsvFieldInfo>();
            var fields = type.GetFields(flags);

            foreach(var field in fields)
            {
                CsvFieldAttribute? attribute = field.GetCustomAttribute<CsvFieldAttribute>();

                result.Add(new CsvFieldInfo
                {
                    Field = field,
                    Alias = attribute?.Name
                });
            }

            return result;
        }

        private static IEnumerable<CsvPropertyInfo> GetProperties(Type type, BindingFlags flags)
        {
            var result = new List<CsvPropertyInfo>();
            var props = type.GetProperties(flags);

            foreach (var property in props)
            {
                CsvFieldAttribute? attribute = property.GetCustomAttribute<CsvFieldAttribute>();

                result.Add(new CsvPropertyInfo
                {
                    Property = property,
                    Alias = attribute?.Name
                });
            }

            return result;
        }
    }

    internal struct CsvFieldInfo
    {
        public FieldInfo Field { get; set; }

        public string? Alias { get; set; }

        public string GetAliasOrName()
        {
            return Alias ?? Field.Name;
        }
    }

    internal struct CsvPropertyInfo
    {
        public PropertyInfo Property { get; set; }

        public string? Alias { get; set; }

        public string GetAliasOrName()
        {
            return Alias ?? Property.Name;
        }
    }
}
