using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class IEnumerableOfTConverterTests
    {
        static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeIReadOnlyCollectionTest()
        {
            var collection = new IEnumerableWithCount<string>(new List<string> { "Spear", "Sword" }, 2);
            var serialized = CsvConverter.Serialize(collection, Options);

            Assert.AreEqual("item1,item2,Count\nSpear,Sword,2", serialized);
        }

        [Test]
        public void DeserializeIReadOnlyCollectionTest()
        {
            var csv = "item1,item2,Count\nSpear,Sword,2";
            var deserialized = CsvConverter.Deserialize<IEnumerableWithCount<string>>(csv, Options);

            CollectionAssert.AreEqual(new string[] { "Spear", "Sword" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        record IEnumerableWithCount<T>(IEnumerable<T> Items, int Count);
    }
}
