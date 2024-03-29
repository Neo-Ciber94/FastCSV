﻿using NUnit.Framework;
using System.Collections.Immutable;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ImmutableArrayOfTConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var collection = new ImmutableArrayContainer<string>(ImmutableArray.Create(new string[]{ "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual($"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3", serialized);
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = $"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<ImmutableArrayContainer<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword", "Shield" }, deserialized.Items);
            Assert.AreEqual(3, deserialized.Count);
        }

        record ImmutableArrayContainer<T>(ImmutableArray<T> Items, int Count);
    }
}
