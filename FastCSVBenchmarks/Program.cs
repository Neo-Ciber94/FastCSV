
using System;
using System.IO;
using System.Text;
using BenchmarkDotNet.Running;
using FastCSV.Benchmarks;
using FastCSV.Internal;

BenchmarkRunner.Run<Utf16ReaderVsStreamReader>();

//var file = File.Open("example.csv", FileMode.Open);
//using var reader = new Utf16Reader(file);
//StringBuilder sb = new StringBuilder(256);

//while (true)
//{
//    string? s = reader.ReadLine(sb);

//    if (s == null)
//    {
//        break;
//    }

//    Console.WriteLine(s);
//    sb.Clear();
//}