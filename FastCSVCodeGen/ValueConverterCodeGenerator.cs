using System;
using System.Collections.Generic;

namespace FastCSVCodeGen
{
    public class ValueConverterCodeGenerator
    {
        private const string Template = @"#nullable enable

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for <see cref=""{1}""/>.
    /// </summary>
    public class {0}ValueConverter : IValueConverter<{1}>
    {
        public string? Read({1} value)
        {
            return {2};
        }

        public bool TryParse(string? s, out {1} value)
        {
            return {1}.TryParse(s!, out value!);
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
                CodeGenerator.WriteToFile(contents, path, $"{name}ValueConverter", overwrite: true);
            }
        }
    }
}
