using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvConverterDictionaryTests
    {
        [Test]
        public void SerializeNullableToDictionaryTests()
        {
            var values = CsvConverter.SerializeToDictionary(new Nullable<int>(10));
            Assert.AreEqual(values["value"], 10);
        }

        [Test]
        public void SerializeNullToDictionaryTest()
        {
            var values = CsvConverter.SerializeToDictionary(new Nullable<int>());
            Assert.AreEqual(values["value"], null);
        }

        [Test]
        public void DeserializeDictionaryToNullableTest()
        {
            var map = new Dictionary<string, string> { { "value", "20" } };
            var value = CsvConverter.DeserializeFromDictionary(map, typeof(Nullable<int>));

            Assert.AreEqual(20, value);
        }

        [Test]
        public void DeserializeNullDictionaryToNullableTest()
        {
            var map = new Dictionary<string, string> { { "value", "" } };
            var value = CsvConverter.DeserializeFromDictionary(map, typeof(Nullable<int>));

            Assert.AreEqual(null, value);
        }
    }
}
