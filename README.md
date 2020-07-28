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

## Example code
Reading and writing using ``CsvReader`` and ``CsvWriter``.
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

Parsing an entire csv to a specify type using the ``CsvDocument<T>``
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
        // Keep in mind the the 'CsvDocument<T>' don't modify the orignal csv, it only load in memory all its data
        // to write the content back to the csv you can use 'CsvDocument.ToString()' to get the csv data and write it 
        // to a file using a StreamWriter. In a future we will provide a way to write the content to a csv file.
        //
        // NOTE: It's not recomended to use the 'CsvDocument' or 'CsvDocument<T>' to read large files,
        // 'CsvReader' is more suitable for those cases.
        CsvDocument<Person> csv = CsvDocument<Person>.FromPath(CsvFile);

        // Get use the getter 'CsvDocument.Values' to get all the inner data store.
        foreach(Person p in csv.Values)
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
    // We use the 'CsvFieldName' attribute to match the names of the csv file,
    // we could also change the names of the properties or fields to match the names of the csv header
    public sealed class Person
    {
        [CsvFieldName("id")]
        public int ID { get; set; }

        [CsvFieldName("first_name")]
        public string FirstName { get; set; }

        [CsvFieldName("last_name")]
        public string LastName { get; set; }

        [CsvFieldName("email")]
        public string Email { get; set; }

        [CsvFieldName("gender")]
        public Gender Gender { get; set; }

        [CsvFieldName("ip_address")]
        public IPAddress IPAddress { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(ID)}={ID}, {nameof(FirstName)}={FirstName}, {nameof(LastName)}={LastName}, {nameof(Email)}={Email}, {nameof(Gender)}={Gender}, {nameof(IPAddress)}={IPAddress}}}";
        }
    }
}
```