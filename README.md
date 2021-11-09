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
- [CsvConverterOptions](#csv-converter-options)
  - [Naming Convention](#csv-converter-options-naming-convention)
  - [Nested Objects](#csv-converter-options-nested-objects)
    - [Limitations](#csv-converter-options-nested-objects-limitations)
  - [Collections](#csv-converter-options-collections)
  - [Custom Converters](#csv-converter-options-converters)
  - [Type Guesser](#csv-converter-options-type-guesser)
  - [Reflection Provider](#csv-converter-options-reflection)


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

#### Read from file async
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

#### Iterate asynchronously
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
```CsvDocument<T>``` is an in-mermory csv document that allows to write, read, updates and remove typed data from a csv,
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
You can also read csv documents from files into a typed ``CsvDocument<T>``

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

// Only map the necesary colums, use 'CsvConverterOptions.MatchExact = true' 
// to force match all the csv columns
record Person(int Id, string? FirstName, string? LastName, int Age);
```

#### Read and query from file

```csharp
using System;
using System.Linq;
using System.Net;
using FastCSV;

var options = new CsvConverterOptions
{
    NamingConvention = CsvNamingConvention.SnakeCase
};

var document = CsvDocument.FromPath<Person>("example.csv", options)
    .Values
    .Take(500)
    .Where(e => e.Age >= 20 && e.Age <= 40)
    .OrderBy(e => e.Gender)
    .ThenBy(e => e.Age)
    .ToCsvDocument(options);

foreach (Person person in document.Values)
{
    Console.WriteLine(person);
}

enum BinaryGender { Male, Female }

record Person(string? Id, string? FirstName, string? LastName, int Age,  BinaryGender Gender, string? Email, IPAddress? IpAddress);
```

## CsvConverterOptions

```CsvConverterOptions``` is the configuration used during serialization/deserialization of a csv document.

### Naming Convention

The naming convention determines how to read and write the field names of a csv document,
all naming conventions inherit from ```CsvNamingConvention``` class.

```csharp
using System;
using System.IO;
using System.Text;
using FastCSV;

using var stream = new MemoryStream();
using var writter = new CsvWriter(stream, leaveOpen: true);

var options = new CsvConverterOptions
{
    NamingConvention = new UpperCaseNamingConvention()
};

writter.WriteType<Product>(options);
writter.WriteValue(new Product(1, "Red Chair", 500m), options);
writter.WriteValue(new Product(2, "Blue Toaster", 100m), options);
writter.WriteValue(new Product(3, "Green Sofa", 2350m), options);

Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));

class UpperCaseNamingConvention : CsvNamingConvention
{
    public override string Convert(string name)
    {
        return name.ToUpper();
    }
}

record Product(int Id, string Name, decimal Price);
```

**Output**

```bash
ID,NAME,PRICE
1,Red Chair,500
2,Blue Toaster,100
3,Green Sofa,2350
```

### Nested Objects
When serializing or deserializing objects we will try to get a converter to transform a value from an specify type into
a collection of fields to the csv and if a converter is not found for a type an exception will be throw.

For thoses cases you can tell the converter to use a ```NestedObjectHandling``` to flatten those nested objects.

```csharp
using FastCSV;

var options = new CsvConverterOptions
{
    NestedObjectHandling = NestedObjectHandling.Default
};

using var writer = new CsvWriter("output.csv");

writer.WriteType<ShoppingCart>(options);
writer.WriteValue<ShoppingCart>(new ShoppingCart(Id: 1, Quantity: 12, Info: new ProductInfo("Apple", 20)), options);
writer.WriteValue<ShoppingCart>(new ShoppingCart(Id: 2, Quantity: 1, Info: new ProductInfo("Frozen Pizza", 200)), options);
writer.WriteValue<ShoppingCart>(new ShoppingCart(Id: 3, Quantity: 5, Info: new ProductInfo("Chair", 800)), options);

record ShoppingCart(int Id, int Quantity, ProductInfo Info);

record ProductInfo(string Name, decimal Price);
```

*output.csv*
```bash
Id,Quantity,Name,Price
1,12,Apple,20
2,1,Frozen Pizza,200
3,5,Chair,800
```

#### Limitations

Nested object serialization may fail when serializing types with self referencing fields/properties:

```csharp
class Node
{
    public int Value { get; set; }
    public Node? Next { get; set; }
    public Node? Prev { get; set; }
}
```

### Collections
Using the ``CollectionHandling`` you can allow serialize/deserialize collections,
inline in the csv.

The way this is done is using a ``tag`` which identifies the items with a name and index,
starting at 1. The default tag when using ``CollectionHandling.Default`` is ``"item"``.

```csharp
using System;
using FastCSV;
using FastCSV.Utils;

string csv = @"
CartId,item1,item2,item3
1,Apple,Pinnable,Banana
2,Keyboard,Mouse,Monitor
3,Coffee,Tea,Milk
".Trim();

var options = new CsvConverterOptions
{
    CollectionHandling = CollectionHandling.Default
};

var stream = StreamHelper.CreateStreamFromString(csv);
using var reader = new CsvReader(stream);

foreach(var e in reader.ReadAllAs<ShoppingCart>(options))
{
    Console.WriteLine(e);
}

record ShoppingCart(int CartId, string[] Products)
{
    public override string ToString()
    {
        string items = string.Join(", ", Products);
        return $"{nameof(ShoppingCart)} {{ CartId = {CartId}, Products = [ {items} ] }}";
    }
}
```

**Output**

```bash
ShoppingCart { CartId = 1, Products = [ Apple, Pinnable, Banana ] }
ShoppingCart { CartId = 2, Products = [ Keyboard, Mouse, Monitor ] }
ShoppingCart { CartId = 3, Products = [ Coffee, Tea, Milk ] }
```

### Custom Converters

By default when serializing/deserializing an object all the built-in types will be serializer with an built-in converter.

The list of built-in types can be found on: ``FastCSV\Converters\CsvDefaultConverterProvider.BuiltinTypes.g.cs``

But when required you can add your own custom converters to the ``CsvConverterOptions.Converters`` by inheriting from:

- ``ICsvValueConverter`` : Provides a low-level API for convert a from and to a string.
- ``ICsvCustomConverter`` : Provides a simple API to convert a value from and to a string.

```csharp
using System;
using FastCSV;
using FastCSV.Converters;
using FastCSV.Utils;

var csv = @"
Id,Position
1,""(20,-34)""
2,""(1,3)""
3,""(-3,400)""
".Trim();

var options = new CsvConverterOptions
{
    Converters = new []{ new PointConverter() },
};

var stream = StreamHelper.CreateStreamFromString(csv, writable: true);

using(var writer = new CsvWriter(stream, leaveOpen: true))
{
    writer.Write(); // Adds a new line
    writer.WriteValue(new Data(4, new Point(12, 34)), options);
}

stream.Position = 0; // Reset the stream position
using var reader = new CsvReader(stream);

foreach (Data data in reader.ReadAllAs<Data>(options))
{
    Console.WriteLine(data);
}

record Data(int Id, Point Position);

record Point(int X, int Y);

class PointConverter : ICsvCustomConverter<Point>
{
    public string? ConvertFrom(Point value)
    {
        return $"({value.X},{value.Y})";
    }

    public bool ConvertTo(ReadOnlySpan<char> s, out Point value)
    {
        value = default!;
        s = s.Trim();

        if (s.Length < 5 || s[0] != '(' || s[^1] != ')')
        {
            return false;
        }

        s = s[1..^1];

        if (s.Length == 0)
        {
            return false;
        }

        int separator = s.IndexOf(',');
        if (separator == -1)
        {
            return false;
        }

        ReadOnlySpan<char> x = s[..separator].Trim();
        ReadOnlySpan<char> y = s[(separator + 1)..].Trim();

        if (!int.TryParse(x, out int xValue) || !int.TryParse(y, out int yValue))
        {
            return false;
        }

        value = new Point(xValue, yValue);
        return true;
    }
}
```

**Output**
```bash
Data { Id = 1, Position = Point { X = 20, Y = -34 } }
Data { Id = 2, Position = Point { X = 1, Y = 3 } }
Data { Id = 3, Position = Point { X = -3, Y = 400 } }
Data { Id = 4, Position = Point { X = 12, Y = 34 } }
```

**Using the ICsvValueConverter API**

```csharp
using System;
using FastCSV;
using FastCSV.Converters;
using FastCSV.Utils;

var csv = @"
Id,Position
1,""(20,-34)""
2,""(1,3)""
3,""(-3,400)""
".Trim();

var options = new CsvConverterOptions
{
    Converters = new []{ new PointConverter() },
};

var stream = StreamHelper.CreateStreamFromString(csv, writable: true);

using(var writer = new CsvWriter(stream, leaveOpen: true))
{
    writer.Write(); // Adds a new line
    writer.WriteValue(new Data(4, new Point(12, 34)), options);
}

stream.Position = 0; // Reset the stream position
using var reader = new CsvReader(stream);

foreach (Data data in reader.ReadAllAs<Data>(options))
{
    Console.WriteLine(data);
}

record Data(int Id, Point Position);

record Point(int X, int Y);

class PointConverter : ICsvValueConverter<Point>
{
    public bool TrySerialize(Point value, ref CsvSerializeState state)
    {
        state.Write($"({value.X},{value.Y})");
        return true;
    }

    public bool TryDeserialize(out Point value, ref CsvDeserializeState state)
    {
        value = default!;
        ReadOnlySpan<char> s = state.Read();

        s = s.Trim();

        if (s.Length < 5 || s[0] != '(' || s[^1] != ')')
        {
            return false;
        }

        s = s[1..^1];

        if (s.Length == 0)
        {
            return false;
        }

        int separator = s.IndexOf(',');
        if (separator == -1)
        {
            return false;
        }

        ReadOnlySpan<char> x = s[..separator].Trim();
        ReadOnlySpan<char> y = s[(separator + 1)..].Trim();

        if (!int.TryParse(x, out int xValue) || !int.TryParse(y, out int yValue))
        {
            return false;
        }

        value = new Point(xValue, yValue);
        return true;
    }
}
```
