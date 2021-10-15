﻿using BenchmarkDotNet.Attributes;
using FastCSV.Struct;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [MinColumn, MaxColumn]
    public class CsvReaderReadvsReadStructBenchmark
    {
        private const string Records1000 = "1000_records.csv";
        private const string Records10000 = "10000_records.csv";
        private const string Records100000 = "100000_records.csv";

        [Params(Records1000, Records10000, Records100000)]
        public string FilePath { get; set; }

        private CsvReader reader;

        [IterationSetup]
        public void SetupReader()
        {
            reader = new CsvReader(FilePath);
        }

        [IterationCleanup]
        public void CleanupReader()
        {
            reader.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void Read()
        {
            while (true)
            {
                CsvRecord? record = reader.Read();

                if (record == null)
                {
                    break;
                }

                BlackBox(record);
            }
        }

        [Benchmark]
        public void ReadStruct()
        {
            while (true)
            {
                CsvRecordStruct? record = reader.ReadStruct();

                if (record == null)
                {
                    break;
                }

                BlackBox(record);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static T BlackBox<T>(in T value) => value;
    }
}
