using System;
using System.Collections.Generic;
using System.Linq;

namespace FastCSVCodeGen
{
    public static class BuiltInTypeCodeGenerator
    {
        public static void WriteTo_ValueConvertersBuiltInTypes(string path, IReadOnlyDictionary<Type, string> types)
        {
            var codeGen = new CodeGenerator();
            
            codeGen.WriteLine("using System;");
            codeGen.WriteLine("using System.Collections.Generic;");
            codeGen.WriteLine("using FastCSV.Converters.Builtin;\n");

            codeGen.Open("namespace FastCSV.Converters", b =>
            {
                b.Open("public static partial class ValueConverters", b =>
                {
                    b.WriteLine("/// <summary>");
                    b.WriteLine("/// Builtin types converters.");
                    b.WriteLine("/// </summary>");
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

            codeGen.WriteTo(path, "ValueConverters.BuiltinTypes", overwrite: true);
        }

        public static void WriteTo_CsvConverterIsBuiltInType(string path, IReadOnlyDictionary<Type, string> types)
        {
            var codeGen = new CodeGenerator();

            codeGen.WriteLine("using System;\n");

            codeGen.Open("namespace FastCSV", b => 
            {
                b.Open("public static partial class CsvConverter", b =>
                {
                    b.WriteLine("/// <summary>");
                    b.WriteLine("/// Checks if the type can be serialize/deserialize with a builtin converter.");
                    b.WriteLine("/// </summary>");
                    b.WriteLine("/// <param name=\"type\">The type<see cref=\"Type\"/>Type to check.</param>");
                    b.WriteLine("/// <returns>The <c>true</c> if can be serialize/deserialize with a default converter.</returns>");

                    b.Open("internal static bool IsBuiltInType(Type type)", b =>
                    {
                        b.WriteLine("Type nullableType = Nullable.GetUnderlyingType(type);");
                        b.Open("if (nullableType != null)", b =>
                        {
                            b.WriteLine("return IsBuiltInType(nullableType);");
                        });

                        b.WriteLine();

                        Type[] actualTypes = types.Where(t => !t.Key.IsPrimitive).Select(t => t.Key).ToArray();

                        b.WriteLine("return type.IsPrimitive");
                        b.WriteLine("\t|| type.IsEnum");

                        for (var i = 0; i < actualTypes.Length; i++)
                        {
                            Type type = actualTypes[i];

                            if (i == actualTypes.Length - 1)
                            {
                                b.WriteLine($"\t|| type == typeof({type});");
                            }
                            else
                            {
                                b.WriteLine($"\t|| type == typeof({type})");
                            }
                        }
                    });
                });
            });

            codeGen.WriteTo(path, "CsvConverter.IsBuiltInType", overwrite: true);
        }
    }
}