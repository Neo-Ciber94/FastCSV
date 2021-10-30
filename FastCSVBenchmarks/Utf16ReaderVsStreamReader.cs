using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using FastCSV.Internal;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [MaxColumn, MinColumn]
    public class Utf16ReaderVsStreamReader
    {
        private static readonly string FileName = "example.csv";

        [Benchmark(Baseline = true)]
        public void ReadFileStreamReader()
        {
            using var reader = new StreamReader(FileName);

            while(true)
            {
                string? s = reader.ReadLine();

                if (s == null)
                {
                    break;
                }

                BlackBox(s);
            }
        }

        [Benchmark]
        public async Task ReadFileStreamReaderAsync()
        {
            using var reader = new StreamReader(FileName);

            while (true)
            {
                string? s = await reader.ReadLineAsync();

                if (s == null)
                {
                    break;
                }

                BlackBox(s);
            }
        }

        [Benchmark]
        public void ReadFileUtf16StreamReader()
        {
            var file = File.Open(FileName, FileMode.Open);
            using var reader = new Utf16Reader(file);

            while (true)
            {
                string? s = reader.ReadLine();

                if (s == null)
                {
                    break;
                }

                BlackBox(s);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static T BlackBox<T>(T value) => value;
    }
}
