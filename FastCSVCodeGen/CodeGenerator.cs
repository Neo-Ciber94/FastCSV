using System;
using System.IO;
using System.Text;

namespace FastCSVCodeGen
{
    /// <summary>
    /// A code generator helper.
    /// </summary>
    public class CodeGenerator
    {
        private const string GenerateCodeWarning = "////////////////// GENERATED CODE, DO NOT EDIT //////////////////";
            
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly int initialIndentation = 0;
        private int indentation = 0;

        /// <summary>
        /// Value written for indentation, by default is a tabulation.
        /// </summary>
        public string IndentString { get; init; } = "\t";

        /// <summary>
        /// Name of this generator, use when loging output.
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Whether if log the output directory name when writting to a file.
        /// </summary>
        public bool LogOutput { get; init; } = true;

        /// <summary>
        /// Constructs a new <see cref="CodeGenerator"/> with an initial indentation of 0.
        /// </summary>
        public CodeGenerator() : this(0) { }

        /// <summary>
        /// Constructs a new <see cref="CodeGenerator"/> with the specified initial indentation.
        /// </summary>
        /// <param name="initialIndentation">The initial indentation.</param>
        public CodeGenerator(int initialIndentation)
        {
            if (initialIndentation < 0)
            {
                throw new ArgumentException($"Indentation cannot be negative, but was {initialIndentation}");
            }

            this.initialIndentation = initialIndentation;
            this.indentation = initialIndentation;
        }

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

        public void WriteLine()
        {
            WriteIndentation();
            stringBuilder.AppendLine();
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
            indentation = initialIndentation;
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }

        public void WriteTo(string path, string fileName, bool overwrite = false)
        {
            if (Name != null)
            {
                Console.WriteLine(Name);
            }

            WriteToFile(ToString(), path, fileName, LogOutput, overwrite);
        }

        public static void WriteToFile(string content, string path, string fileName, bool log = true, bool overwrite = false)
        {
            content = $"{GenerateCodeWarning}\n\n{content}";

            string destinationFileName = Path.GetFullPath($"{path}/{fileName}.g.cs");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                if (!overwrite)
                {
                    throw new Exception($"{path} already exists, use 'overwrite: true' if want to replace it.");
                }
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