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
        public void ReadFileUtf16Reader()
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

        [Benchmark]
        public void ReadFileUtf16ReaderWithStringBuilder()
        {
            var file = File.Open(FileName, FileMode.Open);
            using var reader = new Utf16Reader(file);
            StringBuilder sb = new StringBuilder(256);

            while (true)
            {
                string? s = reader.ReadLine(sb);

                if (s == null)
                {
                    break;
                }

                BlackBox(s);
                sb.Clear();
            }
        }

        [Benchmark]
        public void ReadFileUtf16ReaderWithStringBuilderNoArrayPool()
        {
            var file = File.Open(FileName, FileMode.Open);
            using var reader = new Utf16Reader(file, useArrayPool: false);
            StringBuilder sb = new StringBuilder(256);

            while (true)
            {
                string? s = reader.ReadLine(sb);

                if (s == null)
                {
                    break;
                }

                BlackBox(s);
                sb.Clear();
            }
        }

        [Benchmark]
        public void ReadFileUtf16ReaderWithStringBuilderNoArrayPoolAndCapacity()
        {
            var file = File.Open(FileName, FileMode.Open);
            using var reader = new Utf16Reader(file, useArrayPool: false, bytesCapacity: 1024);
            StringBuilder sb = new StringBuilder(256);

            while (true)
            {
                string? s = reader.ReadLine(sb);

                if (s == null)
                {
                    break;
                }

                BlackBox(s);
                sb.Clear();
            }
        }

        [Benchmark]
        public void ReadFileUtf16ReaderNoArrayPool()
        {
            var file = File.Open(FileName, FileMode.Open);
            using var reader = new Utf16Reader(file, useArrayPool: false);

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

        [Benchmark]
        [Arguments(64)]
        [Arguments(128)]
        [Arguments(256)]
        [Arguments(512)]
        [Arguments(1024)]
        [Arguments(2048)]
        [Arguments(4096)]
        public void ReadFileUtf16ReaderWithCapacity(int bufferCapacity)
        {
            var file = File.Open(FileName, FileMode.Open);
            using var reader = new Utf16Reader(file, bufferCapacity);

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

        //[Benchmark]
        //public void ReadFileBoxUtf16Reader()
        //{
        //    var file = File.Open(FileName, FileMode.Open);
        //    using var reader = new BoxUtf16Reader(file);

        //    while (true)
        //    {
        //        string? s = reader.ReadLine();

        //        if (s == null)
        //        {
        //            break;
        //        }

        //        BlackBox(s);
        //    }
        //}

        //[Benchmark]
        //[Arguments(64)]
        //[Arguments(128)]
        //[Arguments(256)]
        //[Arguments(512)]
        //[Arguments(1024)]
        //[Arguments(2048)]
        //[Arguments(4096)]
        //public void ReadFileBoxUtf16ReaderWithCapacity(int bufferCapacity)
        //{
        //    var file = File.Open(FileName, FileMode.Open);
        //    using var reader = new BoxUtf16Reader(file, bufferCapacity);

        //    while (true)
        //    {
        //        string? s = reader.ReadLine();

        //        if (s == null)
        //        {
        //            break;
        //        }

        //        BlackBox(s);
        //    }
        //}

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static T BlackBox<T>(T value) => value;
    }
}
