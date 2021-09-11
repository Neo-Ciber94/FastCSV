using System;
using System.IO;
using System.Text;

namespace FastCSVCodeGen
{
    public class CodeGenerator
    {
        private const string GenerateCodeWarning = "////////////////// GENERATED CODE, DO NOT EDIT //////////////////";
            
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private int indentation = 0;

        public string IndentString { get; init; } = "\t";

        public string? Name { get; init; }

        public bool LogOutput { get; init; } = true;

        private void Open(string? starts, bool close, Action<CodeGenerator> action)
        {
            if (starts != null)
            {
                WriteLine(starts);
            }

            WriteLine("{");
            indentation += 1;

            action(this);

            indentation -= 1;
            WriteLine(close? "};" : "}");
        }

        public void Open(string starts, Action<CodeGenerator> action)
        {
            Open(starts, false, action);
        }

        public void Open(Action<CodeGenerator> action)
        {
            Open(null, false, action);
        }

        public void OpenClose(string starts, Action<CodeGenerator> action)
        {
            Open(starts, true, action);
        }

        public void OpenClose(Action<CodeGenerator> action)
        {
            Open(null, true, action);
        }

        public void Write<T>(T value)
        {
            WriteIndentation();
            stringBuilder.Append(value?.ToString() ?? string.Empty);
        }

        public void WriteLine<T>(T value)
        {
            WriteIndentation();
            stringBuilder.AppendLine(value?.ToString() ?? string.Empty);
        }

        public void WriteIndentation()
        {
            for(int i = 0; i < indentation; i++)
            {
                stringBuilder.Append(IndentString);
            }
        }

        public void Clear()
        {
            stringBuilder.Clear();
            indentation = 0;
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }

        public void WriteTo(string path, string fileName)
        {
            if (Name != null)
            {
                Console.WriteLine(Name);
            }

            WriteToFile(ToString(), path, fileName, LogOutput);
        }

        public static void WriteToFile(string content, string path, string fileName, bool log = true)
        {
            content = $"{GenerateCodeWarning}\n\n{content}";

            string destinationFileName = Path.GetFullPath($"{path}/{fileName}.g.cs");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using var writer = new StreamWriter(destinationFileName);
            writer.Write(content);
            writer.Flush();

            if (log)
            {
                Console.WriteLine(destinationFileName);
            }
        }
    }
}