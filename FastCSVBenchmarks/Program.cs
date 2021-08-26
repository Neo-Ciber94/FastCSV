using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using FastCSV.Utils;

namespace FastCSV.Benchmarks
{
    class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<ReadAllVsReadAllAsync>();
        }
    }
}