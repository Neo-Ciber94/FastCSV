using System;
using System.Collections.Generic;

namespace FastCSVCodeGen
{
    public class BuiltInTypeCodeGenerator : CodeGenerator
    {
        public static void WriteValueConverters(string path, IReadOnlyDictionary<Type, string> types)
        {
            var codeGen = new CodeGenerator();
            
            codeGen.WriteLine("using System;");
            codeGen.WriteLine("using System.Collections.Generic;\n");

            codeGen.Open("namespace FastCSV.Converters", b =>
            {
                b.Open("public static partial class ValueConverters", b =>
                {
                    b.OpenClose("private static readonly IReadOnlyDictionary<Type, IValueConverter> BuiltInConverters = new Dictionary<Type, IValueConverter>()", b =>
                    {
                        foreach (var (type, name) in types)
                        {
                            var typeOf = $"typeof({type}),";
                            b.WriteLine($"{{ {typeOf,-40} new {name}ValueConverter() }},");
                        }
                    });
                });
            });

            codeGen.WriteTo(path, "ValueConverters.BuiltinTypes");
        }

        public static void WriteCsvConverterBuiltin(string path, IReadOnlyDictionary<Type, string> types)
        {
        }
    }
}