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
    record Box(int Number, string[] Values);

    class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<ReadAllVsReadAllAsync>();
        }
    }
}