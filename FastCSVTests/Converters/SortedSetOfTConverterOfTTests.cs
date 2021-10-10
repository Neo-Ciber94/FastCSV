using NUnit.Framework;
using System.Collections.Generic;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class SortedSetOfTConverterTests
    {
        static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeIReadOnlyCollectionTest()
        {
            var collection = new Container<string>(new SortedSet<string>(new string[] { "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.True(serialized.StartsWith("item1,item2,item3,Count\n"));
            Assert.True(serialized.Contains("Spear"));
            Assert.True(serialized.Contains("Sword"));
            Assert.True(serialized.Contains("Shield"));
            Assert.True(serialized.Contains("3"));
        }

        [Test]
        public void DeserializeIReadOnlyCollectionTest()
        {
            var csv = "item1,item2,item3,Count\nSpear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<Container<string>>(csv, Options);

            CollectionAssert.Contains(deserialized.Items, "Spear");
            CollectionAssert.Contains(deserialized.Items, "Sword");
            CollectionAssert.Contains(deserialized.Items, "Shield");
            Assert.AreEqual(3, deserialized.Count);
        }

        record Container<T>(SortedSet<T> Items, int Count);
    }
}
