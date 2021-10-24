using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [MaxColumn, MinColumn]
    public class ValueStringBuilderVsStringBuilder
    {
        private static readonly StreamReader Reader = new StreamReader("example.csv");
        public int StringBuilderCapacity { get; set; }

        [Benchmark(Baseline = true)]
        public void ParseWithValueStringBuilder()
        {
            while (true)
            {
                string[]? records = CsvUtility.ParseNextRecord(Reader, CsvFormat.Default);

                if (records == null)
                {
                    break;
                }

                BlackBox(records);
            }
        }

        [Benchmark()]
        public void ParseWithStringBuilderCache()
        {
            while (true)
            {
                string[]? records = CsvUtility.ParseNextRecordStringBuilderCache(Reader, CsvFormat.Default);

                if (records == null)
                {
                    break;
                }

                BlackBox(records);
            }
        }


        [Benchmark]
        [Arguments(16)]
        [Arguments(32)]
        [Arguments(64)]
        [Arguments(128)]
        [Arguments(256)]
        [Arguments(512)]
        public void ParseWithStringBuilder(int capacity)
        {
            while (true)
            {
                string[]? records = CsvUtility.ParseNextRecordStringBuilder(Reader, CsvFormat.Default, capacity);

                if (records == null)
                {
                    break;
                }

                BlackBox(records);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static T BlackBox<T>(T value) => value;
    }
}
