using System;
using System.Diagnostics;
using System.Net;
using FastCSV;

namespace FastCSV.Benchmarks
{
    class Program
    {
        public static void Main()
        {
            const string CsvFile = "../../../example.csv";

            using var reader = new CsvReader(CsvFile);
            foreach (var person in reader.ReadAllAs<Person>())
            {
                Console.WriteLine(person);
            }
        }

        public enum Gender
        {
            Male, Female
        }

        public struct Person
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
}