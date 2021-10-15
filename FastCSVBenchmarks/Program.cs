using BenchmarkDotNet.Running;
using FastCSV.Structs;
using FastCSV.Utils;
using System;
using System.IO;

namespace FastCSV.Benchmarks
{
    class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<CsvReaderVsCsvReaderBufferedBenchmark>();
        }
    }
}