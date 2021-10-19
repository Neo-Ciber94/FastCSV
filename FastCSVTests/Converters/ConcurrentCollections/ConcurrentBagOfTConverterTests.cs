using NUnit.Framework;
using System.Collections.Concurrent;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ConcurrentBagOfTConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var collection = new Container<string>(new ConcurrentBag<string> { "Spear", "Sword", "Shield" }, 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.True(serialized.StartsWith($"item1,item2,item3,Count{System.Environment.NewLine}"));
            Assert.True(serialized.Contains("Spear"));
            Assert.True(serialized.Contains("Sword"));
            Assert.True(serialized.Contains("Shield"));
            Assert.True(serialized.Contains("3"));
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = $"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<Container<string>>(csv, Options);

            CollectionAssert.Contains(deserialized.Items, "Spear");
            CollectionAssert.Contains(deserialized.Items, "Sword");
            CollectionAssert.Contains(deserialized.Items, "Shield");
            Assert.AreEqual(3, deserialized.Count);
        }

        record Container<T>(ConcurrentBag<T> Items, int Count);
    }
}
