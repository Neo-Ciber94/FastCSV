using System;
using System.Collections.Generic;

namespace FastCSVCodeGen
{
    public class CsvConverterCodeGenerator
    {
        private const string Template = @"#nullable enable

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref=""{1}""/>.
    /// </summary>
    public class {0}ValueConverter : ICsvCustomConverter<{1}>
    {
        public string? ConvertFrom({1} value)
        {
            return {2};
        }

        public bool ConvertTo(System.ReadOnlySpan<char> s, out {1} value)
        {
            return {1}.TryParse({3}, out value!);
        }
    }
}
";
        public static void WriteTo(string path, IReadOnlyDictionary<Type, string> types)
        {
            foreach(var (type, name) in types)
            {
                if (type == typeof(string))
                {
                    continue;
                }

                string contents = Template
                    .Replace("{0}", name)
                    .Replace("{1}", type.FullName);

                string valueToString = type switch
                {
                    Type t when t == typeof(bool) => "value? \"true\": \"false\"",
                    _ => "value.ToString()"
                };

                contents = contents.Replace("{2}", valueToString);

                if (!HasTryParseWithSpan(type))
                {
                    contents = contents.Replace("{3}", "s.ToString()");
                }
                else
                {
                    contents = contents.Replace("{3}", "s");
                }

                CodeGenerator.WriteToFile(contents, path, $"{name}ValueConverter", overwrite: true);
            }
        }

        private static bool HasTryParseWithSpan(Type type)
        {
            return type switch
            {
                Type _ when typeof(char) == type => false,
                Type _ when typeof(IntPtr) == type => false,
                Type _ when typeof(UIntPtr) == type => false,
                _ => true,
            };
        }
    }
}
