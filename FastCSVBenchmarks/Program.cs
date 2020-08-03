using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FastCSV.Utils;

namespace FastCSV.Benchmarks
{
    class Program
    {
        public static void Main()
        {
            const string CsvFile = "../../../example.csv";

            using var reader = new CsvReader(CsvFile);

            Console.WriteLine(reader.Header[0..4].IntoString());

            foreach(CsvRecord record in reader.ReadAll().Take(20))
            {
                Console.WriteLine(record[0..4].IntoString());
            }
        }

        public enum Gender
        {
            Male, Female
        }


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
}