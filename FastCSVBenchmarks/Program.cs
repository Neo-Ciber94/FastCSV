using System;
using System.Linq;
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

        public static long MeasureAllocated(Action action)
        {
            GC.Collect();

            long start = GC.GetTotalMemory(false);
            action();
            long end = GC.GetTotalMemory(false);

            return Math.Max(0, end - start);
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

            [CsvFieldName("age")]
            public int Age { get; set; }

            [CsvFieldName("email")]
            public string Email { get; set; }

            [CsvFieldName("gender")]
            public Gender Gender { get; set; }

            [CsvFieldName("ip_address")]
            public IPAddress IPAddress { get; set; }

            public override string ToString()
            {
                return $"{{{nameof(ID)}={ID}, {nameof(FirstName)}={FirstName}, {nameof(LastName)}={LastName}, {nameof(Age)}={Age}, {nameof(Email)}={Email}, {nameof(Gender)}={Gender}, {nameof(IPAddress)}={IPAddress}}}";
            }
        }
    }
}