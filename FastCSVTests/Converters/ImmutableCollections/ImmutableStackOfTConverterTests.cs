using NUnit.Framework;
using System.Collections.Immutable;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ImmutableStackOfTConverterTests
    {
        static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var collection = new ImmutableStackContainer<string>(ImmutableStack.Create(new string[] { "Spear", "Sword", "Shield" }), 3);
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
            var deserialized = CsvConverter.Deserialize<ImmutableStackContainer<string>>(csv, Options);

            CollectionAssert.Contains(deserialized.Items, "Spear");
            CollectionAssert.Contains(deserialized.Items, "Sword");
            CollectionAssert.Contains(deserialized.Items, "Shield");
            Assert.AreEqual(3, deserialized.Count);
        }

        [Test]
        public void IImmutableStackSerializeTest()
        {
            var collection = new IImmutableStackContainer<string>(ImmutableStack.Create(new string[] { "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.True(serialized.StartsWith($"item1,item2,item3,Count{System.Environment.NewLine}"));
            Assert.True(serialized.Contains("Spear"));
            Assert.True(serialized.Contains("Sword"));
            Assert.True(serialized.Contains("Shield"));
            Assert.True(serialized.Contains("3"));
        }

        [Test]
        public void IImmutableStackDeserializeTest()
        {
            var csv = $"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<IImmutableStackContainer<string>>(csv, Options);

            CollectionAssert.Contains(deserialized.Items, "Spear");
            CollectionAssert.Contains(deserialized.Items, "Sword");
            CollectionAssert.Contains(deserialized.Items, "Shield");
            Assert.AreEqual(3, deserialized.Count);
        }

        record ImmutableStackContainer<T>(ImmutableStack<T> Items, int Count);
        record IImmutableStackContainer<T>(IImmutableStack<T> Items, int Count);
    }
}
