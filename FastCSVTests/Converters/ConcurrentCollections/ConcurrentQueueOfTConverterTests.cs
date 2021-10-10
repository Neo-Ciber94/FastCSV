using NUnit.Framework;
using System.Collections.Concurrent;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ConcurrentQueueOfTConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var collection = new Container<string>(new ConcurrentQueue<string>(new string[] { "Spear", "Sword" }), 2);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual("item1,item2,Count\nSpear,Sword,2", serialized);
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = "item1,item2,Count\nSpear,Sword,2";
            var deserialized = CsvConverter.Deserialize<Container<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        record Container<T>(ConcurrentQueue<T> Items, int Count);
    }
}
