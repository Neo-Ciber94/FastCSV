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
    public class CsvConverterDuplicatedNameTests
    {
        [Test]
        public void SerializeDuplicatedNameTest()
        {
            string serialized = CsvConverter.Serialize(new Dup { Value = 10, Other = 22 });

            string expected = $"Value,Value{Environment.NewLine}10,22";
            Assert.AreEqual(expected, serialized);
        }

        [Test]
        public void DeserializeDuplicatedNameTest1()
        {
            string csv = $"Value,Value{Environment.NewLine}4,5";

            Dup value = CsvConverter.Deserialize<Dup>(csv);
            Dup expected = new Dup { Value = 4, Other = 5 };
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void DeserializeDuplicatedNameTest2()
        {
            string csv = $"Value,Other{Environment.NewLine}4,5";

            Assert.Throws<InvalidOperationException>(() => 
            {
                var _ = CsvConverter.Deserialize<Dup>(csv);
            }, message: "Cannot find property 'Value'");
        }

        record Dup
        {
            public int Value { get; set; }

            [CsvField("Value")]
            public int Other { get; set; }
        }
    }
}
