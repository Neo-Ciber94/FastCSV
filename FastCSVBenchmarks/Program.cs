
using FastCSV;
using System;

using var reader = new CsvReader("example.csv");

// Awaits each record
await foreach(CsvRecord record in reader.ReadAllAsync())
{
    Console.WriteLine(record);
}

//using FastCSV;

//using var writer = new CsvWriter("mydata.csv");

//// Write the header with an array
//writer.WriteAll(new string[] { "Id", "Name", "Value" });

//// Write an array of objects
//writer.Write(1, "Baseball", 500m);
//writer.Write(2, "Wood Chair", 2599.99m);
//writer.Write(3, "Red Table", 14000m);

//// Write an object
//writer.WriteValue(new Product(4, "RGB Mouse", 2500m));

//record Product(int Id, string Name, decimal Price);

namespace FastCSV.Benchmarks
{
}
