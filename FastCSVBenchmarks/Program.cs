using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;
using FastCSV;

namespace FastCSV.Benchmarks
{
    class Program
    {
        public static void Main()
        {
            //ReadCsvProgram.Run();

            BenchmarkRunner.Run<CsvReaderReadAllBenchmark>();
        }
    }
}
