using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using FastCSV.Converters;
using FastCSV.Utils;

namespace FastCSV.Benchmarks
{
    record Box(int Number, int[] Values);

    class Program
    {
        public static void Main()
        {
            // BenchmarkRunner.Run<ReadAllVsReadAllAsync>();

            var converter = new EnumerableValueConverter<int>(CsvFormat.Default);

            int[] values = new int[] { 13, 79, 40 };

            Console.WriteLine(ToString(converter.GetHeader(values)));

            string csv = converter.ToStringValue(values);
            Console.WriteLine(csv);
        }

        static string ToString(IEnumerable enumerable)
        {
            var values = enumerable.Cast<object>().Select(e => e?.ToString());
            return "[" + string.Join(", ", values) + "]";
        }
    }
}