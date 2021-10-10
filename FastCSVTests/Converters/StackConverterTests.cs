using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class StackConverterTests
    {
        private readonly static CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var container = new Container(new Stack(new object[]{ 1, true, "hello", 24.05f, 'D', 3500.53m }), 6);
            string result = CsvConverter.Serialize(container, Options);

            Assert.True(result.StartsWith("item1,item2,item3,item4,item5,item6,Count"));
            Assert.True(result.Contains("1"));
            Assert.True(result.Contains("true"));
            Assert.True(result.Contains("hello"));
            Assert.True(result.Contains("24.05"));
            Assert.True(result.Contains("D"));
            Assert.True(result.Contains("3500.53"));
            Assert.True(result.Contains("6"));
        }

        [Test]
        public void DeserializeTest()
        {
            string csv = "item1,item2,item3,item4,item5,item6,Count\n1,true,hello,24.05,D,3500.53,6";
            var result = CsvConverter.Deserialize<Container>(csv, Options);

            Assert.AreEqual(6, result.Count);
            CollectionAssert.Contains(result.Items, 1);
            CollectionAssert.Contains(result.Items, true);
            CollectionAssert.Contains(result.Items, "hello");
            CollectionAssert.Contains(result.Items, 3500.53m);

            CollectionAssert.Contains(result.Items.OfType<char>(), 'D');
            CollectionAssert.Contains(result.Items.OfType<float>(), 24.05f);
        }

        record Container(Stack Items, int Count);
    }
}
