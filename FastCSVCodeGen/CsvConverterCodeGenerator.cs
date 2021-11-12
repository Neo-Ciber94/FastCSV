using System;
using System.Collections.Generic;
using System.Linq;

namespace FastCSVCodeGen
{
    public class CsvConverterCodeGenerator
    {
        private const string Template = @"#nullable enable
        
{imports}

namespace FastCSV.Converters.Builtin
{
    /// <summary>
    /// A value converter for <see cref=""{1}""/>.
    /// </summary>
    internal class {0}ValueConverter : ICsvValueConverter<{1}>
    {
        public bool TrySerialize({1} value, ref CsvSerializeState state)
        {
            state.Write({2});
            return true;
        }

        public bool TryDeserialize(out {1} value, ref CsvDeserializeState state)
        {
            ReadOnlySpan<char> s = state.Read();
            return {1}.TryParse({3}, out value!);
        }
    }
}
";
        public static void WriteTo(string path, IReadOnlyDictionary<Type, string> types)
        {
            var imports = new HashSet<string>
            {
                "System"
            };

            foreach (var (type, name) in types)
            {
                if (type == typeof(string) || type == typeof(object))
                {
                    continue;
                }

                string fullTypeName = type.FullName!;
                int namespaceIdx = fullTypeName.LastIndexOf('.');
                string @namespace = fullTypeName[..namespaceIdx];
                string typeName = fullTypeName[(namespaceIdx + 1)..];

                imports.Add(@namespace);

                string contents = Template
                    .Replace("{0}", name)
                    .Replace("{1}", typeName);

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

                string importsString = string.Join("\r\n", imports.Select(i => $"using {i};"));
                contents = contents.Replace("{imports}", importsString);

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
