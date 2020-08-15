using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Running;
using FastCSV.Utils;

namespace FastCSV.Benchmarks
{
    public static class ExceptionHelper
    {
        public static string GetDidYouMeanMessage(string value, IEnumerable<string> possibleValues, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(nameof(value));
            }

            if (!possibleValues.Any())
            {
                throw new ArgumentException($"{nameof(possibleValues)} cannot be empty");
            }

            if (ignoreCase)
            {
                value = value.ToLower();
            }

            List<string> values = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach(string s in possibleValues)
            {
                string other = ignoreCase ? s.ToLower() : s;

                if(other.Contains(value) || value.Contains(other))
                {
                    values.Add(other);
                }
            }

            if(values.Count > 0)
            {
                sb.Append($"Cannot find `{value}`, did you mean: ");
                if(values.Count == 0)
                {
                    sb.Append(values[0]);
                }
                else
                {
                    for(int i = 0; i < values.Count; i++)
                    {
                        sb.Append(values[i]);

                        if(i < values.Count - 1)
                        {
                            if (i == values.Count - 2)
                            {
                                sb.Append(" or ");
                            }
                            else
                            {
                                sb.Append(", ");
                            }
                        }
                    }
                }

                sb.Append("?");
                return sb.ToString();
            }
            else
            {
                return "Cannot find: " + value;
            }
        }
    }

    class Program
    {
        private const string ProjectPath = "../../../";

        public static void Main()
        {
            //using var reader = new CsvReader(ProjectPath + "example.csv");

            //foreach(var record in reader.ReadAll())
            //{
            //    var e = record.GetValues("first_name".As("name"), "age");
            //    Console.WriteLine(e["name"] + ", " + e["age"]);
            //}


            //Console.WriteLine(ExceptionHelper.GetDidYouMeanMessage("name", new[] { "first_name", "last_name", "middle_name", "age", "salary", "id" }));

        }

        public static Result<int, string> Find(int id)
        {
            if(id > 10)
            {
                return Result.Ok(id);
            }
            else
            {
                return Result.Error("Cannot find the value with id: " + id);
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