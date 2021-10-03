using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class CsvConverterIListTests
    {
        static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeListTest()
        {
            var list = new ListWithCount<string>(new List<string> { "Spear", "Sword" }, 2);
            var serialized = CsvConverter.Serialize(list, Options);

            Assert.AreEqual("item1,item2,Count\nSpear,Sword,2", serialized);
        }

        [Test]
        public void DeserializeListTest()
        {
            var csv = "item1,item2,Count\nSpear,Sword,2";
            var deserialized = CsvConverter.Deserialize<ListWithCount<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        record ListWithCount<T>(List<T> Items, int Count);
    }
}
