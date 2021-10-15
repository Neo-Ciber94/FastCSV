using BenchmarkDotNet.Attributes;
using FastCSV.Structs;
using FastCSV.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [MinColumn, MaxColumn]
    public class CsvReaderVsCsvReaderBufferedBenchmark
    {
        private static readonly string CsvData;
        private static readonly string FileName = "example.csv";

        static CsvReaderVsCsvReaderBufferedBenchmark()
        {
            using var reader = new StreamReader(FileName);
            CsvData = reader.ReadToEnd();
        }

        [Benchmark(Baseline = true)]
        public void ReadFile()
        {
            using var reader = new CsvReader(FileName);

            while (!reader.IsDone)
            {
                var record = reader.Read();
                BlackBox(record);
            }
        }

        [Benchmark]
        public void ReadFileBuffered()
        {
            using var reader = new CsvReaderBuffered(FileName);

            while (!reader.IsDone)
            {
                var record = reader.Read();
                BlackBox(record);
            }
        }

        [Benchmark]
        public void ReadString()
        {
            using var reader = CsvReader.FromStream(StreamHelper.ToMemoryStream(CsvData));

            while (!reader.IsDone)
            {
                var record = reader.Read();
                BlackBox(record);
            }
        }

        [Benchmark]
        public void ReadFileStreamBuffered()
        {
            using var fileStream = File.Open(FileName, FileMode.Open);
            using var reader = CsvReaderBuffered.FromStream(fileStream);

            while (!reader.IsDone)
            {
                var record = reader.Read();
                BlackBox(record);
            }
        }

        [Benchmark]
        public void ReadStringBuffered()
        {
            using var reader = CsvReaderBuffered.FromString(CsvData);

            while (!reader.IsDone)
            {
                var record = reader.Read();
                BlackBox(record);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static T BlackBox<T>(T value) => value;
    }
}
