using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;
using FastCSV;

namespace FastCSV.Benchmarks
{
    public enum Gender
    {
        Male, Female
    }

    public sealed class Data
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

    class Program
    {
        public const string CsvFile = "../../../example.csv";

        public static void Main()
        {
            //ReadCsvProgram.Run();

            //BenchmarkRunner.Run<CsvReaderReadAllBenchmark>();

            var document = CsvDocument<Data>.FromPath(CsvFile);

            foreach(var data in document.Values)
            {
                Console.WriteLine((data.ID, data.FirstName, data.LastName));
            }
        }
    }
}
