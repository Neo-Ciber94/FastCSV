using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [MinColumn, MaxColumn]
    public class CsvReaderReadAllAsBenchmark
    {
        private const string ProjectPath = "../../../../../../../";
        private const string FilePath = ProjectPath + "example.csv";

        public enum Gender { Male, Female }

        public class Person
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public int age { get; set; }
            public string email { get; set; }
            public Gender gender { get; set; }
            public IPAddress ip_address { get; set; }
        }

        [Benchmark(Baseline = true)]
        public void ReadAll()
        {
            using var reader = new CsvReader(FilePath);

            foreach(var record in reader.ReadAllAs<Person>())
            {
                BlackBox(record);
            }
        }

        [Benchmark]
        public void ReadAllWithEnumerator()
        {
            using var reader = new CsvReader(FilePath);

            foreach (var record in reader.ReadAllAsWithEnumerator<Person>())
            {
                BlackBox(record);
            }
        }

        [Benchmark]
        public async Task ReadAllAsync()
        {
            using var reader = new CsvReader(FilePath);

            await foreach (var record in reader.ReadAllAsAsync<Person>())
            {
                BlackBox(record);
            }
        }

        [Benchmark]
        public async Task ReadAllWithAsyncEnumerator()
        {
            using var reader = new CsvReader(FilePath);

            await foreach (var record in reader.ReadAllAsAsyncWithAsyncEnumerator<Person>())
            {
                BlackBox(record);
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static T BlackBox<T>(T value) => value;
    }
}
