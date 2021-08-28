using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace FastCSV
{
    public class CsvSerializerOptions
    {
        internal static CsvSerializerOptions Default { get; } = new CsvSerializerOptions();

        public char Delimiter { get; set; } = CsvFormat.DefaultDelimiter;

        public char Quote { get; set; } = CsvFormat.DefautlQuote;

        public QuoteStyle Style { get; set; } = CsvFormat.DefaultStyle;

        public bool IgnoreWhitespace { get; set; } = true;

        public bool IncludeFields { get; set; } = false;
    }

    public static class CsvSerializer
    {
        public static List<string> GetValues<T>(T value, CsvSerializerOptions? options = null)
        {
            if (value == null)
            {
                throw new ArgumentException(nameof(value));
            }

            options ??= CsvSerializerOptions.Default;
            List<string> result = new List<string>();
            Type type = value!.GetType();

            if (IsSimple(type))
            {
                result.Add(value.ToString()!);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                foreach (object? e in (IEnumerable)value)
                {
                    result.Add(e?.ToString() ?? string.Empty);
                }
            }
            else
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                if (options.IncludeFields)
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

                    if (!fields.Any() && !properties.Any())
                    {
                        throw new ArgumentException($"No public fields or properties available for type {typeof(T)}");
                    }

                    foreach (FieldInfo f in fields)
                    {
                        object? e = f.GetValue(value);
                        result.Add(e?.ToString() ?? string.Empty);
                    }
                }
                else
                {
                    if (!properties.Any())
                    {
                        throw new ArgumentException($"No public properties available for type {typeof(T)}");
                    }
                }

                foreach (PropertyInfo p in properties)
                {
                    object? e = p.GetValue(value);
                    result.Add(e?.ToString() ?? string.Empty);
                }
            }

            return result;
        }

        private static bool IsSimple(Type type) 
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(decimal)
                || type == typeof(string)
                || type == typeof(BigInteger)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(IPAddress)
                || type == typeof(Version)
                || type == typeof(Guid);
        }
    }
}
