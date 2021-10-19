using NUnit.Framework;
using System.Collections;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ArrayListConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var container = new Container(new ArrayList() { 1, true, "hello", 24.05f, 'D', 3500.53m }, 6);
            string result = CsvConverter.Serialize(container, Options);

            Assert.AreEqual($"item1,item2,item3,item4,item5,item6,Count{System.Environment.NewLine}1,true,hello,24.05,D,3500.53,6", result);
        }

        [Test]
        public void DeserializeTest()
        {
            string csv = $"item1,item2,item3,item4,item5,item6,Count{System.Environment.NewLine}1,true,hello,24.05,D,3500.53,6";
            var result = CsvConverter.Deserialize<Container>(csv, Options);

            Assert.AreEqual(6, result.Count);
            CollectionAssert.AreEqual(new ArrayList() { 1, true, "hello", 24.05f, 'D', 3500.53m }, result.Items);
        }

        record Container(ArrayList Items, int Count);
    }
}
