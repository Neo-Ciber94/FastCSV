using NUnit.Framework;
using System.Collections.Immutable;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ImmutableQueueOfTConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var collection = new ImmutableQueueContainer<string>(ImmutableQueue.Create(new string[]{ "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual($"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3", serialized);
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = $"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<ImmutableQueueContainer<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword", "Shield" }, deserialized.Items);
            Assert.AreEqual(3, deserialized.Count);
        }

        [Test]
        public void IImmutableQueueSerializeTest()
        {
            var collection = new IImmutableQueueContainer<string>(ImmutableQueue.Create(new string[]{ "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual($"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3", serialized);
        }

        [Test]
        public void IImmutableQueueDeserializeTest()
        {
            var csv = $"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<IImmutableQueueContainer<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword", "Shield" }, deserialized.Items);
            Assert.AreEqual(3, deserialized.Count);
        }

        record ImmutableQueueContainer<T>(ImmutableQueue<T> Items, int Count);
        record IImmutableQueueContainer<T>(IImmutableQueue<T> Items, int Count);
    }
}
