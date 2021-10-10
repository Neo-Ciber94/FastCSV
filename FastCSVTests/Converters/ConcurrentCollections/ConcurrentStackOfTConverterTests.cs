using NUnit.Framework;
using System.Collections.Concurrent;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ConcurrentStackOfTConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var collection = new Container<string>(new ConcurrentStack<string>(new string[] { "Spear", "Sword", "Shield" }), 2);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.True(serialized.StartsWith("item1,item2,item3,Count\n"));
            Assert.True(serialized.Contains("Spear"));
            Assert.True(serialized.Contains("Sword"));
            Assert.True(serialized.Contains("Shield"));
            Assert.True(serialized.Contains("2"));
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = "item1,item2,item3,Count\nSpear,Sword,Shield,2";
            var deserialized = CsvConverter.Deserialize<Container<string>>(csv, Options);

            CollectionAssert.Contains(deserialized.Items, "Spear");
            CollectionAssert.Contains(deserialized.Items, "Sword");
            CollectionAssert.Contains(deserialized.Items, "Shield");
            Assert.AreEqual(2, deserialized.Count);
        }

        record Container<T>(ConcurrentStack<T> Items, int Count);
    }
}
