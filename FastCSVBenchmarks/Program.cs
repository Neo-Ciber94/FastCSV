using FastCSV;
using System;

var options = new CsvConverterOptions
{
    NamingConvention = CsvNamingConvention.SnakeCase
};

CsvDocument<Person> document = CsvDocument.FromPath<Person>("example.csv", options);

foreach(Person e in document.Values)
{
    Console.WriteLine(e);
}

// Only map the necesary colums, use 'CsvConverterOptions.MatchExact = true' to force match all the csv columns
record Person(int Id, string? FirstName, string? LastName, int Age);

//namespace FastCSV.Benchmarks
//{
//}
