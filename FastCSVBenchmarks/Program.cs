using BenchmarkDotNet.Running;
using FastCSV.Utils;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FastCSV.Benchmarks
{
    class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<CsvReaderReadAllAsBenchmark>();
        }
    }
}