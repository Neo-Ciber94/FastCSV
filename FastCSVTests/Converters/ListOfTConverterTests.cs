using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ListOfTConverterTests
    {
        static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeListTest()
        {
            var list = new ListWithCount<string>(new List<string> { "Spear", "Sword" }, 2);
            var serialized = CsvConverter.Serialize(list, Options);

            Assert.AreEqual($"item1,item2,Count{System.Environment.NewLine}Spear,Sword,2", serialized);
        }

        [Test]
        public void DeserializeListTest()
        {
            var csv = $"item1,item2,Count{System.Environment.NewLine}Spear,Sword,2";
            var deserialized = CsvConverter.Deserialize<ListWithCount<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        [Test]
        public void SerializeIListTest()
        {
            var list = new IListWithCount<string>(new List<string> { "Spear", "Sword" }, 2);
            var serialized = CsvConverter.Serialize(list, Options);

            Assert.AreEqual($"item1,item2,Count{System.Environment.NewLine}Spear,Sword,2", serialized);
        }

        [Test]
        public void DeserializeIListTest()
        {
            var csv = $"item1,item2,Count{System.Environment.NewLine}Spear,Sword,2";
            var deserialized = CsvConverter.Deserialize<IListWithCount<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        [Test]
        public void SerializeIReadOnlyListTest()
        {
            var list = new IReadOnlyListWithCount<string>(new List<string> { "Spear", "Sword" }, 2);
            var serialized = CsvConverter.Serialize(list, Options);

            Assert.AreEqual($"item1,item2,Count{System.Environment.NewLine}Spear,Sword,2", serialized);
        }

        [Test]
        public void DeserializeIReadOnlyListTest()
        {
            var csv = $"item1,item2,Count{System.Environment.NewLine}Spear,Sword,2";
            var deserialized = CsvConverter.Deserialize<IReadOnlyListWithCount<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        record ListWithCount<T>(List<T> Items, int Count);

        record IListWithCount<T>(IList<T> Items, int Count);

        record IReadOnlyListWithCount<T>(IReadOnlyList<T> Items, int Count);
    }
}
