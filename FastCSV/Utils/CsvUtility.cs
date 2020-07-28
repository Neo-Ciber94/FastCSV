using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace FastCSV.Utils
{
    /// <summary>
    /// Stores the results of a parsing operation.
    /// </summary>
    public struct ParseResult
    {
        /// <summary>
        /// A failed parse result.
        /// </summary>
        public static readonly ParseResult Failed = new ParseResult();

        private ParseResult(object? result)
        {
            Result = result;
            Success = true;
        }

        /// <summary>
        /// Gets a successful parsing result with the given value.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static ParseResult Ok(object? result)
        {
            return new ParseResult(result);
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public object? Result { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the parse operation is successful.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the parse is successful; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; }
    }

    /// <summary>
    /// Delegate used by <see cref="CsvUtility.CreateInstance{T}(Dictionary{string, string}, ParserDelegate)"/>.
    /// </summary>
    /// <param name="key">The name of the field or property being parse.</param>
    /// <param name="value">The string value of the property to parse.</param>
    /// <returns>The result of the parsing operation.</returns>
    public delegate ParseResult ParserDelegate(string key, string value);

    public static class CsvUtility
    {
        public static List<string>? ReadRecord(StreamReader reader, CsvFormat format)
        {
            return ReadRecord(new StringBuilder(), reader, format);
        }

        public static List<string>? ReadRecord(StringBuilder stringBuilder, StreamReader reader, CsvFormat format)
        {
            if (reader.EndOfStream)
            {
                return null;
            }

            List<string> records = new List<string>();
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
            return ToCsvString(new StringBuilder(), values, format);
        }

        public static string ToCsvString(StringBuilder stringBuilder, IEnumerable<string> values, CsvFormat format)
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
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

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
                var fields = GetFields(type, BindingFlags.Public | BindingFlags.Instance);
                var props = GetProperties(type, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                if (!fields.Any() && !props.Any())
                {
                    throw new ArgumentException($"Not public fields or properties available for type {typeof(T)}");
                }

                foreach (var f in fields)
                {
                    result.Add(f.GetAliasOrName());
                }

                foreach (var p in props)
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
                object? obj = null;

                if (parser != null)
                {
                    ParseResult parseResult = parser(key, value);

                    if (parseResult.Success)
                    {
                        obj = parseResult.Result;
                    }
                    else
                    {
                        if (!TryParse(value, type, out obj))
                        {
                            throw new InvalidOperationException($"Unable to initalize property: {fieldOrPropertyName}");
                        }
                    }
                }
                else
                {
                    if (!TryParse(value, type, out obj))
                    {
                        throw new InvalidOperationException($"Unable to initalize property: {fieldOrPropertyName}");
                    }
                }

                return obj;
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
                else
                {
                    throw new InvalidOperationException("Unable to cast object of type: " + type.Name);
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
                CsvFieldNameAttribute? attribute = field.GetCustomAttribute<CsvFieldNameAttribute>();

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
                CsvFieldNameAttribute? attribute = property.GetCustomAttribute<CsvFieldNameAttribute>();

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
