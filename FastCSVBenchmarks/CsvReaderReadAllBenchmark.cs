using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastCSV.Benchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [MinColumn, MaxColumn]
    public class CsvReaderReadAllBenchmark
    {
        private const string CsvPath = "../../../../../../../example.csv";

        [Benchmark(Baseline = true)]
        public void ReadAllWithStringBuilder()
        {
            using var reader = new CsvReader(CsvPath);

            foreach(CsvRecord record in reader.ReadAll())
            {
                var _ = record;
            }
        }
    }
}
