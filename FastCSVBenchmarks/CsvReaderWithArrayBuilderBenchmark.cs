using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [MinColumn, MaxColumn]
    public class CsvReaderWithArrayBuilderBenchmark
    {
        private static readonly string ProjectPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
        private static readonly string FilePath = ProjectPath + "example.csv";
        private static string CsvStringData;

        static CsvReaderWithArrayBuilderBenchmark()
        {
            using var reader = new StreamReader(FilePath);
            CsvStringData = reader.ReadToEnd();
        }

        [Benchmark(Baseline = true)]
        public void CsvReadFile()
        {
            using var reader = new CsvReader(FilePath);
        }

        [Benchmark]
        public void CsvReadWithArrayBuilder()
        {
            using var reader = new CsvReader(FilePath);
        }
    }
}
