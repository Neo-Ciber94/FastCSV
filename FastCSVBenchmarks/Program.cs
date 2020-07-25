using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV;

namespace FastCSV.Benchmarks
{
    class Program
    {
        const string ProjectPath = "../../../";

        static void Main()
        {
            using var reader = new CsvReader(ProjectPath + "example.csv");
            Console.WriteLine(reader.Header);

            foreach (var r in reader.ReadAll())
            {
                Console.WriteLine(r);
            }

            foreach (var r in reader.ReadAll())
            {
                Console.WriteLine(r);
            }
        }

        public static void Print<T>(Span<T> span)
        {
            Print(span.ToArray());
        }

        public static void Print<T>(T value)
        {
            Console.WriteLine(ToString(value));
        }

        public static string ToString<T>(T value)
        {
            if(value == null)
            {
                return "null";
            }

            if(value is IEnumerable enumerable)
            {
                StringBuilder sb = new StringBuilder();
                IEnumerator enumerator = enumerable.GetEnumerator();

                if(enumerator.MoveNext())
                {
                    while (true)
                    {
                        string s = enumerator.Current?.ToString()?? "null";
                        sb.Append(s);

                        if (enumerator.MoveNext())
                        {
                            sb.Append(",");
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return sb.ToString();
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
