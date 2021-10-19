# FastCSV

A CSharp library for read and write csv documents.

## Features
- Read csv documents using ``CsvReader``.
- Write csv documents using ``CsvWriter`` and ``CsvDocument``.
- Parse objects to csv using ``CsvConverter``.
- Changed the format of read/write using ``CsvFormat``.

## Table of content
- [Read CSV](#read-csv)
  - [Read from file](#read-file)
  - [Read from string](#read-string)
  - [Iterate over records](#iterate-records)
  - [Read Asynchronously](#read-async)
    - [Read from file async](#read-async)
    - [Iterate asynchronously](#iterate-async-records)
- [Write CSV](#write-csv)
  - [Write to file](#write-file)
  - [Write to Stream](#write-stream)

### Read CSV

#### Read from file
```csharp
using FastCSV;
using System;

using var reader = new CsvReader("example.csv");

// Reads the header
Console.WriteLine(reader.Header);

while (true)
{
    // Reads the next available record
    CsvRecord? record = reader.Read();

    if (record == null)
    {
        break;
    }

    Console.WriteLine(record);
}
```

#### Read from string
```csharp
using FastCSV;
using FastCSV.Utils;
using System;

// Create a Stream from the stream,
// 'StreamHelper' is a utility class from the library
using var stream = StreamHelper.CreateStreamFromString(@"
    id,name,age
    1,Alan,20
    2,Maria,18
    3,Karen,34"
);

using var reader = CsvReader.FromStream(stream);

// Reads the header
Console.WriteLine(reader.Header);

while (true)
{
    // Reads the next record
    CsvRecord? record = reader.Read();

    if (record == null)
    {
        break;
    }

    Console.WriteLine(record);
}
```

**Output**
```bash
id,name,age
1,Alan,20
2,Maria,18
3,Karen,34
```

#### Iterate over records
```csharp
using FastCSV;
using System;

using var reader = new CsvReader("example.csv");

// ReadAll() returns an enumerable that read each record
foreach(CsvRecord record in reader.ReadAll())
{
    Console.WriteLine(record);
}
```

#### Read Asynchronously
**FastCSV** also provides support for ``async-await``

##### Read from file async
```csharp
using FastCSV;
using System;

using var reader = new CsvReader("example.csv");

while (true)
{
    // Awaits the next record
    CsvRecord? record = await reader.ReadAsync();

    if (record == null)
    {
        break;
    }

    Console.WriteLine(record);
}
```

##### Iterate asynchronously
```csharp
using FastCSV;
using System;

using var reader = new CsvReader("example.csv");

// Awaits each record
await foreach(CsvRecord record in reader.ReadAllAsync())
{
    Console.WriteLine(record);
}
```

### Write CSV
```csharp
using FastCSV;

using var writer = new CsvWriter("mydata.csv");

// Write the header with an array
writer.WriteAll(new string[] { "Id", "Name", "Value" });

// Write an array of objects
writer.Write(1, "Baseball", 500m);
writer.Write(2, "Wood Chair", 2599.99m);
writer.Write(3, "Red Table", 14000m);

// Write an object
writer.WriteValue(new Product(4, "RGB Mouse", 2500m));

//
record Product(int Id, string Name, decimal Price);
```