
using System;
using System.IO;

namespace FastCSVCodeGen
{
    class Program
    {
        static void Main()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string path = @$"{basePath}..\..\..\..\FastCSV\\Converters\\";
            ValueConverterCodeGenerator.WriteTo(Path.GetFullPath(path));

            Console.ReadKey();
        }
    }
}
