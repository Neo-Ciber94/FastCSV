﻿using NUnit.Framework;
using System.Collections.Generic;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class LinkedListOfTConverterTests
    {
        static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeIReadOnlyCollectionTest()
        {
            var collection = new Container<string>(new LinkedList<string>(new string[] { "Spear", "Sword", "Shield" }), 3);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual($"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3", serialized);
        }

        [Test]
        public void DeserializeIReadOnlyCollectionTest()
        {
            var csv = $"item1,item2,item3,Count{System.Environment.NewLine}Spear,Sword,Shield,3";
            var deserialized = CsvConverter.Deserialize<Container<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword", "Shield" }, deserialized.Items);
            Assert.AreEqual(3, deserialized.Count);
        }

        record Container<T>(LinkedList<T> Items, int Count);
    }
}
