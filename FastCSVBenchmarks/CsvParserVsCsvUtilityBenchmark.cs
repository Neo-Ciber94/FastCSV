using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [LongRunJob]
    [MaxColumn, MinColumn]
    public class CsvParserVsCsvUtilityBenchmark
    {
        private const string FileName = "example.csv";

        public StreamReader Reader { get; set; } = default!;

        [IterationSetup]
        public void Setup()
        {
            Reader = new StreamReader(FileName);
        }

        [IterationCleanup]
        public void Cleanup()
        {
            Reader.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void CsvUtility_ParseNextRecord()
        {
            while (!Reader.EndOfStream)
            {
                var s = CsvUtility.ParseNextRecord(Reader, CsvFormat.Default);
                BlackBox(s);
            }
        }

        [Benchmark]
        public void CsvParser_ParseNext()
        {
            using var parser = new CsvParser(Reader, CsvFormat.Default);

            while (!parser.IsDone)
            {
                var s = parser.ParseNext();
                BlackBox(s);
            }
        }

        [Benchmark]
        public async Task CsvUtility_ParseNextRecordAsync()
        {
            while (!Reader.EndOfStream)
            {
                var s = await CsvUtility.ParseNextRecordAsync(Reader, CsvFormat.Default);
                BlackBox(s);
            }
        }

        [Benchmark]
        public async Task CsvParser_ParseNextAsync()
        {
            using var parser = new CsvParser(Reader, CsvFormat.Default);

            while (!parser.IsDone)
            {
                var s = await parser.ParseNextAsync();
                BlackBox(s);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static T BlackBox<T>(T value) => value;
    }
}
