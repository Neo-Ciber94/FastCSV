using NUnit.Framework;
using System.Collections.Generic;

namespace FastCSV.Converters
{
    [TestFixture]
    class CsvConverterICollectionTests
    {
        static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeICollectionTest()
        {
            var collection = new ICollectionWithCount<string>(new List<string> { "Spear", "Sword" }, 2);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual("item1,item2,Count\nSpear,Sword,2", serialized);
        }

        [Test]
        public void DeserializeICollectionTest()
        {
            var csv = "item1,item2,Count\nSpear,Sword,2";
            var deserialized = CsvConverter.Deserialize<ICollectionWithCount<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        [Test]
        public void SerializeIReadOnlyCollectionTest()
        {
            var collection = new IReadOnlyCollectionWithCount<string>(new List<string> { "Spear", "Sword" }, 2);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual("item1,item2,Count\nSpear,Sword,2", serialized);
        }

        [Test]
        public void DeserializeIReadOnlyCollectionTest()
        {
            var csv = "item1,item2,Count\nSpear,Sword,2";
            var deserialized = CsvConverter.Deserialize<IReadOnlyCollectionWithCount<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        record ICollectionWithCount<T>(ICollection<T> Items, int Count);

        record IReadOnlyCollectionWithCount<T>(IReadOnlyCollection<T> Items, int Count);
    }
}
