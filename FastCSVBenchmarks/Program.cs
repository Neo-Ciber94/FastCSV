using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using BenchmarkDotNet.Running;
using FastCSV.Utils;

namespace FastCSV.Benchmarks
{
    public struct IndexedValue<T>
    {
        public T Value { get; }
        public int Index { get; }

        public IndexedValue(T value, int index)
        {
            Value = value;
            Index = index;
        }

        public override string ToString()
        {
            return $"{{{nameof(Value)}={Value}, {nameof(Index)}={Index}}}";
        }
    }

    class Program
    {
        private const string ProjectPath = "../../../";

        public static void Main()
        {
            //const string CsvFile = ProjectPath + "example.csv";
            //using var reader = new CsvReader(CsvFile);

            //string s = CsvUtility.ToPrettyString(reader.ReadAll().ToList().Take(20));
            //Console.WriteLine(s);

            using(var reader = new CsvReader(ProjectPath + "example.csv"))
            {
                CsvHeader header = reader.Header;
                IEnumerable<CsvRecord> records = reader.ReadAll().Skip(10).Take(10);
                CsvDocument csv = CsvDocument.FromRaw(header, records);

                Console.WriteLine(csv.ToPrettyString());
            }
            //BenchmarkRunner.Run<ReadAllVsReadAllAsync>();
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