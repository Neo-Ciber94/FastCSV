using System;
using System.Linq;
using System.Net;
using FastCSV;

var options = new CsvConverterOptions
{
    NamingConvention = CsvNamingConvention.SnakeCase
};

//var document = CsvDocument.FromPath<Person>("example.csv", options)
//    .Values
//    .Skip(10)
//    .Take(500)
//    .Where(e => e.Age >= 20 && e.Age <= 40)
//    .OrderBy(e => e.Gender)
//    .ThenBy(e => e.Age)
//    .ToCsvDocument(options);

var document = CsvDocument.FromPath<Person>("example.csv", options);

foreach (var record in document.Values)
{
    Console.WriteLine(record);
}

enum BinaryGender { Male, Female }

record Person(string? Id, string? FirstName, string? LastName, int Age,  BinaryGender Gender, string? Email, IPAddress? IpAddress);
