# FastCSV

A CSharp library for read and write csv documents.

## Features
- Read csv documents using ``CsvReader``.
- Write csv documents using ``CsvWriter`` and ``CsvDocument``.
- Parse objects to csv using ``CsvConverter``.
- Changed the format of read/write using ``CsvFormat``.

## Table of contents
- [Read CSV](#read-csv)
  - [Read from file](#read-file)
  - [Read from string](#read-string)
  - [Iterate over records](#iterate-records)
  - [Read typed data](#read-typed)
  - [Read typed using a naming convention](#read-typed-naming-contention)
  - [Read Asynchronously](#read-async)
    - [Read from file async](#read-async)
    - [Iterate asynchronously](#iterate-async-records)
- [Write CSV](#write-csv)
  - [Write to file](#write-file)
  - [Write to Stream](#write-stream)
- [CsvDocument](#csv-document)
- [CsvDocument\<T\>](#csv-document-typed)
  - [Write into CsvDocument\<T\>](#csv-document-typed-write)
  - [Read csv from file](#csv-document-read-file)


> The examples are written using C# 'top-level statements'
## Read CSV

###  Read from file
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

###  Read from string
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

###  Iterate over records
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

###  Read typed data
```csharp
using FastCSV;
using System;
using System.Net;

using var reader = new CsvReader("example.csv");

// You can use 'ReadAs' ans 'ReadAllAs' to read the data as a specific type
foreach(Person person in reader.ReadAllAs<Person>())
{
    Console.WriteLine(person);
}

enum BinaryGender { Male, Female }

// The model that represents the data, we use 'CsvField' to match the name of the document
record Person
{
    [CsvField("id")]
    public string? Id { get; set; }

    [CsvField("first_name")]
    public string? FirstName { get; set; }

    [CsvField("last_name")]
    public string? LastName { get; set; }

    [CsvField("age")]
    public int Age { get; set; }
    
    [CsvField("gender")]
    public BinaryGender Gender { get; set; }

    [CsvField("email")]
    public string? Email { get; set; }

    [CsvField("ip_address")]
    public IPAddress? IPAddress { get; set; }
}
```

###  Read typed using a naming convention
```csharp
using FastCSV;
using System;
using System.Net;

using var reader = new CsvReader("example.csv");

// CsvConverterOptions allow us to modify how we convert objects to csv
var options = new CsvConverterOptions
{
    NamingConvention = CsvNamingConvention.SnakeCase
};

foreach(Person person in reader.ReadAllAs<Person>(options))
{
    Console.WriteLine(person);
}

enum BinaryGender { Male, Female }

record Person(string? Id, string? FirstName, string? LastName, int Age, BinaryGender Gender, string? Email, IPAddress? IpAddress);
```

###  Read Asynchronously
**FastCSV** also provides support for ``async-await``

### # Read from file async
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

### # Iterate asynchronously
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

## Write CSV

###  Write to file
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

###  Write to Stream
```csharp
using FastCSV;
using System;
using System.IO;

using var memoryStream = new MemoryStream(128);
using var writer = CsvWriter.FromStream(memoryStream, leaveOpen: true);

// Write the header with an array
writer.WriteAll(new string[] { "Id", "Name", "Value" });

// Write an array of objects
writer.Write(1, "Baseball", 500m);
writer.Write(2, "Wood Chair", 2599.99m);
writer.Write(3, "Red Table", 14000m);

// Write an object
writer.WriteValue(new Product(4, "RGB Mouse", 2500m));

// Reset the position to read
memoryStream.Position = 0;

// Reads the stream
using var streamReader = new StreamReader(memoryStream);
Console.WriteLine(streamReader.ReadToEnd());

//
record Product(int Id, string Name, decimal Price);
```

**Output**
```bash
Id,Name,Value
1,Baseball,500
2,Wood Chair,2599.99
3,Red Table,14000
4,RGB Mouse,2500
```

### CsvDocument
```CsvDocument``` is an in-memory csv document and allows add, update and remove the values.

```csharp
using FastCSV;
using System;

// Creates a document with a header,
// Flexible documents can have an irregular number of columns
var document = new CsvDocument(new string[] { "id", "name", "price" }, flexible: true);

// Write record as an array of strings
document.WriteAll(new string[] { "1", "Keyboard", "2500.99" });

// Write an array of objects
document.Write(2, "Mouse RGB", 1200m, "A colorful rgb mouse!");

// Writes a typed value
document.WriteValue(new Product(4, "USB Cable", 50m));

// Write at specify index
document.WriteAt(2, new string[] { "3", "Tea Leaves" });

// Updates with a typed value
document.UpdateValue(3, new Product(4, "USB Cable Type C", 50m));

Console.WriteLine(document);

record Product(int Id, string? Name, decimal Price);
```

**Output**
```bash
id,name,price
1,Keyboard,2500.99
2,Mouse RGB,1200,A colorful rgb mouse!
3,Tea Leaves
4,USB Cable Type C,50
```

**Why use CsvDocument over CsvWriter?**
```CsvWriter``` only allow write to a ```Stream``` which can be a file, but
```CsvDocument``` allow to write, read and update which is more versatile.

### CsvDocument\<T\>
```CsvDocument<T>``` is an in-mermory csv document that allows to write, read, updates and remove typed data into a csv,
also provides operations to query over the document.

#### Write into CsvDocument\<T\>
```csharp
using FastCSV;
using System;

var document = new CsvDocument<Product>();

// Adds the values
document.Write(new Product(1, "Hamburger", 10m));
document.Write(new Product(2, "Steak", 350m));
document.Write(new Product(3, "Hot Chicken Wings", 500m));

// Updates a value
document.Update(1, new Product(2, "Pork", 300m));

Console.WriteLine(document);

// Writes the data to a file
await document.CopyToFileAsync("mydata.csv");

record Product(int Id, string? Name, decimal Price);
```

#### Read csv from file
You can also read csv documents from files into a typed ``CsvDocument\<T>```

```csharp
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
```