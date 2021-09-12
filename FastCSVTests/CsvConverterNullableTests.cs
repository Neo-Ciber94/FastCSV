using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvConverterNullableTests
    {
        [Test]
        public void SerializeNullableTest()
        {
            var csv = CsvConverter.Serialize(new Nullable<int>(10), typeof(Nullable<int>));
            Assert.AreEqual("value\n10", csv);
        }

        [Test]
        public void SerializeNullTest()
        {
            var csv = CsvConverter.Serialize(null, typeof(Nullable<int>));
            Assert.AreEqual("value\n", csv);
        }

        [Test]
        public void DeserializeNullableTest()
        {
            Nullable<int> value = CsvConverter.Deserialize<Nullable<int>>("value\n22");
            Assert.AreEqual(new Nullable<int>(22), value);
        }

        [Test]
        public void DeserializeNullTest()
        {
            Nullable<int> value = CsvConverter.Deserialize<Nullable<int>>("value\n");
            Assert.AreEqual(new Nullable<int>(), value);
        }

        ///////////// No header

        [Test]
        public void SerializeNullableNoHeaderTest()
        {
            var csv = CsvConverter.Serialize(new Nullable<int>(10), typeof(Nullable<int>), new CsvConverterOptions { IncludeHeader = false });
            Assert.AreEqual("10", csv);
        }

        [Test]
        public void SerializeNullNoHeaderTest()
        {
            var csv = CsvConverter.Serialize(null, typeof(Nullable<int>), new CsvConverterOptions { IncludeHeader = false });
            Assert.AreEqual("", csv);
        }

        [Test]
        public void DeserializeNullableNoHeaderTest()
        {
            Nullable<int> value = CsvConverter.Deserialize<Nullable<int>>("22", new CsvConverterOptions { IncludeHeader = false });
            Assert.AreEqual(new Nullable<int>(22), value);
        }
    }
}
