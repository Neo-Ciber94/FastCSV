
using System;
using System.IO;
using FastCSV.Internal;

var file = File.Open("example.csv", FileMode.Open);
using var reader = new Utf16Reader(file);

while (true)
{
    string? s = reader.ReadLine();

    if (s == null)
    {
        break;
    }

    Console.WriteLine(s);
}