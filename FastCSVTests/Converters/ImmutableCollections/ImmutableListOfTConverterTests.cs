using NUnit.Framework;
using System.Collections.Immutable;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ImmutableListOfTConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var collection = new ImmutableListContainer<string>(ImmutableList.Create(new string[] { "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual("item1,item2,item3,Count\nSpear,Sword,Shield,3", serialized);
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = "item1,item2,item3,Count\nSpear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<ImmutableListContainer<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword", "Shield" }, deserialized.Items);
            Assert.AreEqual(3, deserialized.Count);
        }

        [Test]
        public void IImmutableListSerializeTest()
        {
            var collection = new IImmutableListContainer<string>(ImmutableList.Create(new string[] { "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual("item1,item2,item3,Count\nSpear,Sword,Shield,3", serialized);
        }

        [Test]
        public void IImmutableListDeserializeTest()
        {
            var csv = "item1,item2,item3,Count\nSpear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<IImmutableListContainer<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword", "Shield" }, deserialized.Items);
            Assert.AreEqual(3, deserialized.Count);
        }

        record ImmutableListContainer<T>(ImmutableList<T> Items, int Count);
        record IImmutableListContainer<T>(IImmutableList<T> Items, int Count);
    }
}
