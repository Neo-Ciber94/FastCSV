using NUnit.Framework;
using System.Collections.Concurrent;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class BlockingCollectionOfTConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var collection = new Container<string>(new BlockingCollection<string> { "Spear", "Sword" }, 2);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual($"item1,item2,Count{System.Environment.NewLine}Spear,Sword,2", serialized);
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = $"item1,item2,Count{System.Environment.NewLine}Spear,Sword,2";
            var deserialized = CsvConverter.Deserialize<Container<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        record Container<T>(BlockingCollection<T> Items, int Count);
    }
}
