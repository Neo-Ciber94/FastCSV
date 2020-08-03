﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization;

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

            using ValueStringBuilder stringBuilder = new ValueStringBuilder(stackalloc char[128]);

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
                    result.Add(e?.ToString() ?? string.Empty);
                }
            }
            else
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                if (!fields.Any() && !props.Any())
                {
                    throw new ArgumentException($"Not public fields or properties available for type {typeof(T)}");
                }

                foreach (FieldInfo f in fields)
                {
                    object? e = f.GetValue(value);
                    result.Add(e?.ToString() ?? string.Empty);
                }

                foreach (PropertyInfo p in props)
                {
                    object? e = p.GetValue(value);
                    result.Add(e?.ToString() ?? string.Empty);
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
                var fields = GetFields(type, BindingFlags.Public | BindingFlags.Instance);
                var props = GetProperties(type, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                if (!fields.Any() && !props.Any())
                {
                    throw new ArgumentException($"Not public fields or properties available for type {typeof(T)}");
                }

                foreach (CsvFieldInfo f in fields)
                {
                    result.Add(f.GetAliasOrName());
                }

                foreach (CsvPropertyInfo p in props)
                {
                    result.Add(p.GetAliasOrName());
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
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                if (!fields.Any() && !props.Any())
                {
                    throw new ArgumentException($"Not public fields or properties available for type {typeof(T)}");
                }

                foreach (FieldInfo f in fields)
                {
                    object? e = f.GetValue(value);
                    result.Add(f.Name, e);
                }

                foreach (PropertyInfo p in props)
                {
                    object? e = p.GetValue(value);
                    result.Add(p.Name, e);
                }
            }

            return result;
        }

        public static T CreateInstance<T>(Dictionary<string, string> data)
        {
            return CreateInstance<T>(data, null);
        }

        public static T CreateInstance<T>(Dictionary<string, string> data, ParserDelegate? parser)
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

                    object? obj = ParseValue(field.FieldType, field.Name, pair.Key, pair.Value);
                    field.SetValue(result.Value, obj);
                }

                PropertyInfo? prop = GetProperty(type, pair.Key, BindingFlags.Public | BindingFlags.Instance)?.Property;

                if (prop != null)
                {
                    if (!result.HasValue)
                    {
                        result = Optional.Some(FormatterServices.GetUninitializedObject(type));
                    }

                    object? obj = ParseValue(prop.PropertyType, prop.Name, pair.Key, pair.Value);
                    prop.SetValue(result.Value, obj);
                }

                if(field == null && prop == null)
                {
                    throw new InvalidOperationException("Cannot find a field or property named: " +pair.Key);
                }
            }

            if (!result.HasValue)
            {
                throw new InvalidOperationException($"Unable to initializated an object of type {typeof(T)}");
            }

            return (T)result.Value;

            // Helper method
            object? ParseValue(Type type, string fieldOrPropertyName, string key, string value)
            {
                if(parser != null)
                {
                    ParseResult parseResult = parser(key, value);

                    if (parseResult.IsSuccess)
                    {
                        return parseResult.Result;
                    }
                }

                if (!TryParse(value, type, out object? result))
                {
                    throw new InvalidOperationException($"Unable to initalize property: {fieldOrPropertyName}");
                }

                return result;
            }
        }

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

        public static bool TryParse(string value, Type type, out object? result)
        {
            result = null;

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
                else if (type == typeof(decimal))
                {
                    bool ret = decimal.TryParse(value, out var obj);
                    result = obj;
                    return ret;
                }
            }
            else
            {
                if (type == typeof(BigInteger))
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

        public static MemoryStream ToStream(string data)
        {
            var memory = new MemoryStream(data.Length);

            using(var writer = new StreamWriter(memory, leaveOpen: true))
            {
                writer.Write(data);
                writer.Flush();
                memory.Position = 0;
            }

            return memory;
        }

        public static string ToPrettyString(IEnumerable<CsvRecord> records)
        {
            if (!records.Any())
            {
                return string.Empty;
            }

            const int MinPadding = 5;

            CsvHeader? header = records.First().Header;

            using ValueStringBuilder recordBuilder = new ValueStringBuilder(stackalloc char[64]);
            using ValueStringBuilder resultBuilder = new ValueStringBuilder(stackalloc char[256]);
            int columns = header?.Length ?? records.First().Length;

            Span<int> columnSizes = columns < 100? stackalloc int[columns]: new int[columns];

            // Takes the header lenghts
            if(header != null)
            {
                for (int i = 0; i < columns; i++)
                {
                    columnSizes[i] = header[i].Length;
                }
            }

            foreach (CsvRecord record in records)
            {
                Debug.Assert(record.Header == header);

                for(int i = 0; i < columns; i++)
                {
                    columnSizes[i] = Math.Max(columnSizes[i], record[i].Length);
                }
            }

            if (header != null)
            {
                for(int i = 0; i < columns; i++)
                {
                    string field = header[i];

                    if(i < columns - 1)
                    {
                        field = field.PadRight(columnSizes[i] + MinPadding);
                    }

                    recordBuilder.Append(field);
                }

                resultBuilder.AppendLine(recordBuilder.ToString());
                recordBuilder.Clear();
            }

            foreach(CsvRecord record in records)
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
