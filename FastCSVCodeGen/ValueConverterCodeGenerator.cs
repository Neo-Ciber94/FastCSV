using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;

namespace FastCSVCodeGen
{
    public class ValueConverterCodeGenerator
    {
        private const string Template = @"#nullable enable

////////////////// GENERATED CODE, DO NOT EDIT //////////////////

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref=""{1}""/>.
    /// </summary>
    public class {0}ValueConverter : IValueConverter<{1}>
    {
        public string ToValue({1} value)
        {
            return {2};
        }

        public bool TryParse(string? s, out {1} value)
        {
            return {1}.TryParse(s, out value);
        }
    }
}
";
        private static readonly IReadOnlyDictionary<Type, string> Types = new Dictionary<Type, string>
        {
            { typeof(bool), "Bool" },
            { typeof(char), "Char" },
            { typeof(byte), "Byte" },
            { typeof(short), "Short" },
            { typeof(int), "Int" },
            { typeof(long), "Long" },
            { typeof(float), "Float" },
            { typeof(double), "Double" },
            { typeof(sbyte), "SByte" },
            { typeof(ushort), "UShort" },
            { typeof(uint), "UInt" },
            { typeof(ulong), "ULong" },
            { typeof(decimal), "Decimal" },
            { typeof(Half), nameof(Half) },
            { typeof(DateTime), nameof(DateTime) },
            { typeof(DateTimeOffset), nameof(DateTimeOffset) },
            { typeof(BigInteger), nameof(BigInteger) },
            { typeof(Guid), nameof(Guid) },
            { typeof(Version), nameof(Version) },
            { typeof(TimeSpan), nameof(TimeSpan) },
            { typeof(IPAddress), nameof(IPAddress) },
        };

        public static void WriteTo(string path)
        {
            WriteToInternal(path, Types);
        }

        private static void WriteToInternal(string path, IReadOnlyDictionary<Type, string> types)
        {
            foreach(var (type, name) in types)
            {
                string contents = Template
                    .Replace("{0}", name)
                    .Replace("{1}", type.FullName);

                string valueToString = type switch
                {
                    Type t when t == typeof(bool) => "value? \"true\": \"false\"",
                    _ => "value.ToString()"
                };

                contents = contents.Replace("{2}", valueToString);

                string fileName = $"{path}/{name}ValueConverter.g.cs";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var writer = new StreamWriter(fileName))
                {
                    writer.WriteLine(contents);
                    Console.WriteLine(fileName);
                }
            }
        }
    }
}
