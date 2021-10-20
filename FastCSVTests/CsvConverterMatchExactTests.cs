using FastCSV.Collections;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV
{
    [TestFixture]
    public class CsvConverterMatchExactTests
    {
        private static readonly CsvConverterOptions Options = new CsvConverterOptions { MatchExact = true };

        [Test]
        public void DeserializeCsvNoExactTest()
        {
            string csv = @"Id,FirstName,LastName,Age,Email,Gemder,IpAddress
1,Romeo,Abela,34,rabela0@usatoday.com,Male,217.15.244.208";

            Person product = CsvConverter.Deserialize<Person>(csv);
            Assert.AreEqual(new Person(1, "Romeo", "Abela"), product);
        }

        [Test]
        public void DeserializeDictionaryNoExactTest()
        {
            var data = new Dictionary<string, SingleOrList<string>>
            {
                {"Id", "2" },
                {"FirstName", "Horatius" },
                {"LastName", "Garner" },
                {"Age", "51" },
                {"Email", "hgarner1@163.com," },
                {"IpAddress",  "30.236.135.5"}
            };

            Person product = CsvConverter.DeserializeFromDictionary<Person>(data);
            Assert.AreEqual(new Person(2, "Horatius", "Garner"), product);
        }

        [Test]
        public void DeserializeExactTest()
        {
            string csv = @"Id,FirstName,LastName,Age,Email,Gemder,IpAddress
1,Romeo,Abela,34,rabela0@usatoday.com,Male,217.15.244.208";

            Assert.Throws<InvalidOperationException>(() =>
            {
                Person product = CsvConverter.Deserialize<Person>(csv, Options);
            });
        }

        [Test]
        public void DeserializeDictionaryExactTest()
        {
            var data = new Dictionary<string, SingleOrList<string>>
            {
                {"Id", "2" },
                {"FirstName", "Horatius" },
                {"LastName", "Garner" },
                {"Age", "51" },
                {"Email", "hgarner1@163.com," },
                {"IpAddress",  "30.236.135.5"}
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                Person product = CsvConverter.DeserializeFromDictionary<Person>(data, Options);
            });
        }

        record Person(int Id, string FirstName, string LastName);
    }
}
