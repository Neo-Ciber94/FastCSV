using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvConverterRenamingTests
    {
        [Test]
        public void SerializeWithRenamingTest()
        {
            string serialized = CsvConverter.Serialize(new A { Value = 10, Other = 20 });

            string expected = $"Other,Value{Environment.NewLine}10,20";
            Assert.AreEqual(expected, serialized);
        }

        [Test]
        public void DeserializeWithRenamingTest()
        {
            string csv = $"Other,Value{Environment.NewLine}12,23";

            A value = new A { Value = 12, Other = 23 };
            Assert.AreEqual(value, CsvConverter.Deserialize<A>(csv));
        }

        record A
        {
            [CsvField("Other")]
            public int Value { get; set; }

            [CsvField("Value")]
            public int Other { get; set; }
        }
    }
}
