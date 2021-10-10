using NUnit.Framework;
using System.Collections.Immutable;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ImmutableHashSetOfTConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var collection = new ImmutableHashSetContainer<string>(ImmutableHashSet.Create(new string[]{ "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.True(serialized.StartsWith("item1,item2,item3,Count\n"));
            Assert.True(serialized.Contains("Spear"));
            Assert.True(serialized.Contains("Sword"));
            Assert.True(serialized.Contains("Shield"));
            Assert.True(serialized.Contains("3"));
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = "item1,item2,item3,Count\nSpear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<ImmutableHashSetContainer<string>>(csv, Options);

            CollectionAssert.Contains(deserialized.Items, "Spear");
            CollectionAssert.Contains(deserialized.Items, "Sword");
            CollectionAssert.Contains(deserialized.Items, "Shield");
            Assert.AreEqual(3, deserialized.Count);
        }

        [Test]
        public void IImmutableSetSerializeTest()
        {
            var collection = new IImmutableSetContainer<string>(ImmutableHashSet.Create(new string[]{ "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.True(serialized.StartsWith("item1,item2,item3,Count\n"));
            Assert.True(serialized.Contains("Spear"));
            Assert.True(serialized.Contains("Sword"));
            Assert.True(serialized.Contains("Shield"));
            Assert.True(serialized.Contains("3"));
        }

        [Test]
        public void  IImmutableSetDeserializeTest()
        {
            var csv = "item1,item2,item3,Count\nSpear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<IImmutableSetContainer<string>>(csv, Options);

            CollectionAssert.Contains(deserialized.Items, "Spear");
            CollectionAssert.Contains(deserialized.Items, "Sword");
            CollectionAssert.Contains(deserialized.Items, "Shield");
            Assert.AreEqual(3, deserialized.Count);
        }

        record ImmutableHashSetContainer<T>(ImmutableHashSet<T> Items, int Count);
        record IImmutableSetContainer<T>(IImmutableSet<T> Items, int Count);
    }
}
