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

    public readonly struct Result<T, E>
    {
        private readonly object? _value;
        private readonly bool _isOk;

        private Result(T value, E error, bool isOk)
        {
            _value = isOk ? (object)value : error;
            _isOk = isOk;
        } 

        public static Result<T, E> Ok(T value)
        {
            return new Result<T, E>(value, default, isOk: true);
        }

        public static Result<T, E> Error(E error)
        {
            return new Result<T, E>(default, error, isOk: false);
        }
    }
}
