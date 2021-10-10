using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    public class ArrayConverterTests
    {
        private static readonly CsvConverterOptions Options = new CsvConverterOptions
        {
            CollectionHandling = CollectionHandling.Default
        };

        [Test]
        public void SerializeObjectWithArrayTest()
        {
            var obj = new ListWithCount(new string[] { "Apple", "Chips", "Chicken" }, 3);
            var csv = CsvConverter.Serialize(obj, Options);

            Assert.AreEqual("item1,item2,item3,Count\nApple,Chips,Chicken,3", csv);
        }

        [Test]
        public void DeserializeObjectWithArrayTest()
        {
            var csv = "item1,item2,item3,Count\nApple,Chips,Chicken,3";
            var obj = CsvConverter.Deserialize<ListWithCount>(csv, Options);

            var other = new ListWithCount(new string[] { "Apple", "Chips", "Chicken" }, 3);
            Assert.AreEqual(other.Items, obj.Items);
            Assert.AreEqual(other.Count, obj.Count);
        }

        [Test]
        public void SerializeObjectWithArrayTest2()
        {
            var obj = new CountWithList(3, new string[] { "Apple", "Chips", "Chicken" });
            var csv = CsvConverter.Serialize(obj, Options);

            Assert.AreEqual("Count,item1,item2,item3\n3,Apple,Chips,Chicken", csv);
        }

        [Test]
        public void DeserializeObjectWithArrayTest2()
        {
            var csv = "Count,item1,item2,item3\n3,Apple,Chips,Chicken";
            var obj = CsvConverter.Deserialize<CountWithList>(csv, Options);

            var other = new CountWithList(3, new string[] { "Apple", "Chips", "Chicken" });
            Assert.AreEqual(other.Items, obj.Items);
            Assert.AreEqual(other.Count, obj.Count);
        }

        [Test]
        public void SerializeObjectWithArrayTest3()
        {
            var obj = new IndexWithListCount(1, new string[] { "Apple", "Chips", "Chicken" }, 3);
            var csv = CsvConverter.Serialize(obj, Options);

            Assert.AreEqual("Index,item1,item2,item3,Count\n1,Apple,Chips,Chicken,3", csv);
        }

        [Test]
        public void DeserializeObjectWithArrayTest3()
        {
            var csv = "Index,item1,item2,item3,Count\n1,Apple,Chips,Chicken,3";
            var obj = CsvConverter.Deserialize<IndexWithListCount>(csv, Options);

            var other = new IndexWithListCount(1, new string[] { "Apple", "Chips", "Chicken" }, 3);
            Assert.AreEqual(other.Items, obj.Items);
            Assert.AreEqual(other.Count, obj.Count);
        }

        record ListWithCount(string[] Items, int Count);

        record CountWithList(int Count, string[] Items);

        record IndexWithListCount(int Index, string[] Items, int Count);
    }
}
