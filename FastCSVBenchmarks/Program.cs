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

namespace FastCSV.Benchmarks
{

}