using BenchmarkDotNet.Attributes;

namespace FastCSV.Benchmarks
{
    [MemoryDiagnoser]
    [MinColumn, MaxColumn]
    public class DeserializeRecursionVsIteration
    {
        private static readonly CsvConverterOptions DefaultOptions = new() { NestedObjectHandling = NestedObjectHandling.Default };
        private const string Csv = "Value\n10";

        [Benchmark(Baseline = true)]
        public object DeserializeRecursive()
        {
            return CsvConverter.DeserializeRecursive(Csv, typeof(A), DefaultOptions);
        }

        [Benchmark]
        public object DeserializeValueStack()
        {
            return CsvConverter.DeserializeValueStack(Csv, typeof(A), DefaultOptions);
        }

        [Benchmark]
        public object DeserializeIteration()
        {
            return CsvConverter.Deserialize(Csv, typeof(A), DefaultOptions);
        }

        public record A(B B);
        public record B(C C);
        public record C(D D);
        public record D(E E);
        public record E(F F);
        public record F(G G);
        public record G(H H);
        public record H(int Value);
    }
}
