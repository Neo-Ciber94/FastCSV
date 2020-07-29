# FastCSV

A CSharp library for read and write csv documents.

## Contents
The FastCSV library contains the follow classes:

* ``CsvReader``: For read csv from a *path*, *StreamReader* or *Stream*.
* ``CsvWriter``: Which allow writes to csv from a *path*, *StreamWriter* or *Stream*.
* ``CsvDocument``: Represents an in-memory csv document and allows write, update and read.
* ``CsvDocument<T>``: Represents a typed in-memory csv document, allow write, update and read elements of type ``T``.

Also provides:
* ``CsvRecord``: Represents a read-only csv record and provides indexing, slicing and enumeration over its fields.
* ``CsvHeader``: Represents a read-only csv header and provides indexing, slicing and enumeration over its fields.
* ``CsvFormat``: Provides the format used in a csv document: the delimiter, quote and quote style.

## Examples

##### Example 1: Reading and Writing using CsvReader and CsvWriter
```csharp
using System;
using FastCSV;

class Program
{
    public enum Gender
    {
        Male, Female
    }

    public static void Main()
    {
        const string CsvFile = "../../../example.csv";

        // Creates a writer to the file.
        // Internally it use a StreamWriter so need to be disposed in this case with a 'using' statement.
        using (CsvWriter writer = new CsvWriter(CsvFile))
        {
            // Writes a record, the method 'Write' takes an array of objects,
            // so it box the passed values, you can also use 'CsvWriter.WriteAll' which take an 'IEnumerable<string>'.
            writer.Write(1001, "Frida", "Kahlo", "frida@hotmail.com", Gender.Female, "220.30.1.245");
        }

        // Creates a reader to the file 'example.csv'
        // Like the CsvWriter it need to be disposed, so we use a 'using' statement.
        using (CsvReader reader = new CsvReader(CsvFile))
        {
            // The first line is considered the header.
            // If the csv don't have a header use 'hasHeader: false' in the CsvReader constructor
            // and the first line will be considered other record.
            Console.WriteLine(reader.Header);

            // Reads all the records using 'CsvReader.ReadAll' which returns a lazy
            // evaluated enumerator over the records of the csv
            foreach (CsvRecord record in reader.ReadAll())
            {
                Console.WriteLine(record);
            }
        }
    }
}
```

##### Example 2: Parsing an entire csv file to a specify type using the CsvDocument\<T\>
```csharp
using System;
using System.Net;
using FastCSV;

class Program
{
    public static void Main()
    {
        const string CsvFile = "../../../example.csv";

        // We can read a CSV file parsing all its data to a specify type, in this case the type 'Person'
        // this supports the primitive types (bool, int, long, float...) and other types as
        // string, enums, BigInteger, TimeSpan, DateTime, DateTimeOffset, Guid, IPAddress and Version.
        //
        // We can also provide parsing for other types using the 'ParserDelegate'.
        //
        // Keep in mind the the 'CsvDocument<T>' don't modify the orignal csv, it only load in-memory all its data,
        // to write to a file use 'CsvDocument<T>.WriteContentsToFile(...)'
        //
        // NOTE: It's not recomended to use the 'CsvDocument' or 'CsvDocument<T>' to read large files,
        // 'CsvReader' is more suitable for those cases.
        CsvDocument<Person> csv = CsvDocument<Person>.FromPath(CsvFile);

        // Get use the getter 'CsvDocument.Values' to get all the inner data store.
        foreach (Person p in csv.Values)
        {
            Console.WriteLine(p);
        }
    }

    public enum Gender
    {
        Male, Female
    }

    // Class used to parse the data to.
    //
    // We use the 'CsvField' attribute to match the names of the csv file,
    // we could also change the names of the properties or fields to match the names of the csv header
    public sealed class Person
    {
        [CsvField("id")]
        public int ID { get; set; }

        [CsvField("first_name")]
        public string FirstName { get; set; }

        [CsvField("last_name")]
        public string LastName { get; set; }

        [CsvField("age")]
        public int Age { get; set; }

        [CsvField("email")]
        public string Email { get; set; }

        [CsvField("gender")]
        public Gender Gender { get; set; }

        [CsvField("ip_address")]
        public IPAddress IPAddress { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(ID)}={ID}, {nameof(FirstName)}={FirstName}, {nameof(LastName)}={LastName}, {nameof(Age)}={Age}, {nameof(Email)}={Email}, {nameof(Gender)}={Gender}, {nameof(IPAddress)}={IPAddress}}}";
        }
    }
}
```

##### Example 3: Query over the records using 'ReadAllAs<T\>'

```csharp
using System;
using System.Linq;
using System.Net;
using FastCSV;

class Program
{
    public static void Main()
    {
        const string CsvFile = "../../../example.csv";

        // Creates a CsvReader to the file 'example.csv'
        using var reader = new CsvReader(CsvFile);

        // Using 'ReadAllAs<Person>' we get a enumerable over the records,
        // each of which is converted to the type 'Person'.
        // 
        // Also we can query the elements using LINQ expressions
        var result = from person in reader.ReadAllAs<Person>()
                     where person.Age >= 18 && person.Age <= 30
                     where person.Gender == Gender.Female
                     orderby person.Age
                     select new { person.ID, person.FirstName, person.Age, person.Gender };

        foreach (var p in result.Take(20))
        {
            Console.WriteLine(p);
        }
    }

    public enum Gender
    {
        Male, Female
    }

    // Class used to parse the data to.
    //
    // We use the 'CsvField' attribute to match the names of the csv file,
    // we could also change the names of the properties or fields to match the names of the csv header
    public class Person
    {
        [CsvField("id")]
        public int ID { get; set; }

        [CsvField("first_name")]
        public string FirstName { get; set; }

        [CsvField("last_name")]
        public string LastName { get; set; }

        [CsvField("age")]
        public int Age { get; set; }

        [CsvField("email")]
        public string Email { get; set; }

        [CsvField("gender")]
        public Gender Gender { get; set; }

        [CsvField("ip_address")]
        public IPAddress IPAddress { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(ID)}={ID}, {nameof(FirstName)}={FirstName}, {nameof(LastName)}={LastName}, {nameof(Age)}={Age}, {nameof(Email)}={Email}, {nameof(Gender)}={Gender}, {nameof(IPAddress)}={IPAddress}}}";
        }
    }
}
```