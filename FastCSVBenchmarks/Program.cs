using BenchmarkDotNet.Running;
using FastCSV.Utils;
using System;
using System.IO;

namespace FastCSV.Benchmarks
{
    class Program
    {
        public static void Main()
        {
            // BenchmarkRunner.Run<ReadAllVsReadAllAsync>();

            string data = "Name,Age\n" +
                "Homer , 35\n" +
                " Marge,28\n";

            using var memory = new MemoryStream(data.Length * 2);
            using var writer = new StreamWriter(memory, leaveOpen: true);
            writer.Write(data);
            writer.Flush();
            memory.Position = 0;

            using var streamReader = new StreamReader(memory);
            using var reader = new CsvBufferedReader(memory);

            while (true)
            {
                var s = reader.ReadRecord(CsvFormat.Default);

                if (s.Length == 0)
                {
                    break;
                }

                Console.WriteLine($"{IntoString(s)}  {s.Length}");
            }
        }

        private static string IntoString(string[] array) => $"[{string.Join(",", array)}]";
    }
}