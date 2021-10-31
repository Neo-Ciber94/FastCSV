

using BenchmarkDotNet.Running;
using FastCSV.Benchmarks;

BenchmarkRunner.Run<CsvParserVsCsvUtilityBenchmark>();