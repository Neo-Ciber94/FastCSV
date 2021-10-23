using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastCSV.Benchmarks
{
    [LongRunJob]
    [MemoryDiagnoser]
    [MaxColumn, MinColumn]
    public class ParseNextVsParseNextCoreBenchmark
    {
        private static readonly StreamReader reader = new StreamReader("example.csv");

        [Benchmark(Baseline = true)]
        public List<string[]> ParseNext()
        {
            var records = new List<string[]>();

            while (true)
            {
                var result = CsvUtility.ParseNextRecord(reader, CsvFormat.Default);
                if (result == null)
                {
                    break;
                }

                records.Add(result);
            }

            return records;
        }

        [Benchmark]
        public List<string[]> ParseNextCore()
        {
            var records = new List<string[]>();

            while (true)
            {
                if (reader.EndOfStream)
                {
                    break;
                }

                CsvUtility.ParseNextRecordCore(reader, CsvFormat.Default, ref records, OnRead);
            }

            static void OnRead(Span<string?> data, ref List<string[]> list)
            {
                if (data != null)
                {
                    list.Add(data.ToArray()!);
                }
            }

            return records;
        }
    }
}
