using BenchmarkDotNet.Running;

namespace FastCSV.Benchmarks
{
    class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<CsvReaderWithArrayBuilderBenchmark>();
        }
    }
}