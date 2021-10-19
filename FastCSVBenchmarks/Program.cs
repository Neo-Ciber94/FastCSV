using FastCSV;
using System;

var options = new CsvConverterOptions
{
    NamingConvention = CsvNamingConvention.SnakeCase
};

CsvDocument<Person> document = CsvDocument.FromPath<Person>("example.csv", options);

foreach(var e in document.Values)
{
    Console.WriteLine(e);
}

record Person(int Id, string? FirstName, string? LastName, int Age);

//namespace FastCSV.Benchmarks
//{
//}
