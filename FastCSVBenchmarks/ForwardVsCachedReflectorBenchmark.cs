using System.Net;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using FastCSV.Internal;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [MaxColumn, MinColumn]
    public class ForwardVsCachedReflectorBenchmark
    {
        private static readonly string FileName = "example.csv";

        private static readonly CsvConverterOptions ForwardOptions = new()
        {
            NamingConvention = CsvNamingConvention.SnakeCase,
            ReflectionProvider = ForwardReflector.Default
        };

        private static readonly CsvConverterOptions CachedOptions = new()
        {
            NamingConvention = CsvNamingConvention.SnakeCase,
            ReflectionProvider = CachedReflector.Default
        };


        [Benchmark(Baseline = true)]
        public void ForwardReflectorReflection()
        {
            using var reader = new CsvReader(FileName);

            foreach(var value in reader.ReadAllAs<Person>(ForwardOptions))
            {
                BlackBox(value);
            }
        }

        [Benchmark()]
        public void CachedReflectorReflection()
        {
            using var reader = new CsvReader(FileName);

            foreach (var value in reader.ReadAllAs<Person>(CachedOptions))
            {
                BlackBox(value);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static T BlackBox<T>(T value) => value;
    }

    enum BinaryGender { Male, Female }

    record Person(int Id, string FirstName, string LastName, int Age, string Email, BinaryGender Gender, IPAddress IpAddress);
}
