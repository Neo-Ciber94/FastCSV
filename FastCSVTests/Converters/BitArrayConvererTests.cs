using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters
{
    [TestFixture]
    class BitArrayConvererTests
    {
        private static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTest()
        {
            var bits = new BitsContainer(new BitArray(new bool[] { true, false, true, false, true, true }));
            var serialized = CsvConverter.Serialize(bits, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5,item6\n1,0,1,0,1,1", serialized);
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = "item1,item2,item3,item4,item5,item6\n1,0,1,0,1,1";
            var deserialized = CsvConverter.Deserialize<BitsContainer>(csv, Options);

            CollectionAssert.AreEqual(new BitArray(new bool[] { true, false, true, false, true, true }), deserialized.Bits);
        }

        record BitsContainer(BitArray Bits);
    }
}
