using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [MinColumn, MaxColumn]
    public class ReadAllVsReadAllAsync
    {
        private const string ProjectPath = "../../../../../../../";
        private const string FilePath = ProjectPath + "example.csv";

        [Benchmark(Baseline = true)]
        public void ReadAll()
        {
            using var reader = new CsvReader(FilePath);

            foreach(var record in reader.ReadAll())
            {
                var _ = record;
            }
        }

        [Benchmark]
        public async Task ReadAllAsync()
        {
            using var reader = new CsvReader(FilePath);

            await foreach (var record in reader.ReadAllAsync())
            {
                var _ = record;
            }
        }
    }
}
