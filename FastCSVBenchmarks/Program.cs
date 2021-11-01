using System;
using BenchmarkDotNet.Running;
using FastCSV;
using FastCSV.Benchmarks;
using FastCSV.Internal;

BenchmarkRunner.Run<ForwardVsCachedReflectorBenchmark>();
