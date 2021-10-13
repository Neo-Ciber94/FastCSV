using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
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
        private const string ProjectPath = "../../../../../../../";
        private const string FilePath = ProjectPath + "example.csv";
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

            while (true)
            {
                var record = reader.Read();

                if (record == null)
                {
                    break;
                }
            }
        }

        [Benchmark]
        public void CsvReadFileWithBuilder()
        {
            using var reader = new CsvReader(FilePath);

            while (true)
            {
                var record = reader.ReadWithBuilder();

                if (record == null)
                {
                    break;
                }
            }
        }
    }
}
