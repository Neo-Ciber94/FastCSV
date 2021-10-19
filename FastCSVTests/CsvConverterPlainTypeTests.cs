using FastCSV.Converters;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvConverterPlainTypeTests
    {
        [Test]
        public void SerializeAndDeserializeCustomTypeTest()
        {
            var options = new CsvConverterOptions
            {
                Converters = new List<ICsvValueConverter> { new OddOrEvenNumberConverter() }
            };

            var n = new OddOrEvenNumber(34);

            string serialized = CsvConverter.Serialize(n, options);
            Assert.AreEqual($"value{System.Environment.NewLine}34:even", serialized);

            OddOrEvenNumber deserialized = CsvConverter.Deserialize<OddOrEvenNumber>(serialized, options);
            Assert.AreEqual(n, deserialized);
        }

        [Test]
        public void SerializeAndDeserializeCustomTypeNoHeaderTest()
        {
            var options = new CsvConverterOptions
            {
                Converters = new List<ICsvValueConverter> { new OddOrEvenNumberConverter() },
                IncludeHeader = false
            };

            var n = new OddOrEvenNumber(34);

            string serialized = CsvConverter.Serialize(n, options);
            Assert.AreEqual("34:even", serialized);

            OddOrEvenNumber deserialized = CsvConverter.Deserialize<OddOrEvenNumber>(serialized, options);
            Assert.AreEqual(n, deserialized);
        }

        [Test]
        public void SerializeDeserializeCustomTypeCollectionTest()
        {
            var array = new OddOrEvenNumber[]
            {
                new (21),
                new (50),
                new (82)
            };

            var options = new CsvConverterOptions
            {
                Converters = new List<ICsvValueConverter> { new OddOrEvenNumberConverter() },
                CollectionHandling = CollectionHandling.Default
            };

            var serialized = CsvConverter.Serialize(array, options);
            Assert.AreEqual($"item1,item2,item3{System.Environment.NewLine}21:odd,50:even,82:even", serialized);

            var deserialized = CsvConverter.Deserialize<OddOrEvenNumber[]>(serialized, options);
            CollectionAssert.AreEqual(array, deserialized);
        }

        [Test]
        public void SerializeDeserializeCustomTypeCollectionNoHeaderTest()
        {
            var array = new OddOrEvenNumber[]
            {
                new (21),
                new (50),
                new (82),
            };

            var options = new CsvConverterOptions
            {
                Converters = new List<ICsvValueConverter> { new OddOrEvenNumberConverter() },
                CollectionHandling = CollectionHandling.Default,
                IncludeHeader = false
            };

            var serialized = CsvConverter.Serialize(array, options);

            Assert.AreEqual("21:odd,50:even,82:even", serialized);
            Assert.Throws<InvalidOperationException>(() =>
            {
                var deserialized = CsvConverter.Deserialize<OddOrEvenNumber[]>(serialized, options);
            });
        }

        [Test]
        public void SerializeAndDeserializeBuiltinTypeCollectionTest()
        {
            var array = new int[] { 1, 2, 3, 4, 5 };
            var options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

            var serialized = CsvConverter.Serialize(array, options);
            Assert.AreEqual($"item1,item2,item3,item4,item5{System.Environment.NewLine}1,2,3,4,5", serialized);

            var deserialized = CsvConverter.Deserialize<int[]>(serialized, options);
            CollectionAssert.AreEqual(array, deserialized);
        }

        record OddOrEvenNumber(int Value) 
        {
            public bool IsEven => Value % 2 == 0;

            public override string ToString()
            {
                return IsEven ? $"{Value}:even" : $"{Value}:odd";
            }
        }

        class OddOrEvenNumberConverter : ICsvCustomConverter<OddOrEvenNumber>
        {
            public string ConvertFrom(OddOrEvenNumber value)
            {
                return value.ToString();
            }

            public bool ConvertTo(ReadOnlySpan<char> s, out OddOrEvenNumber value)
            {
                value = default;
                int separatorIndex = s.IndexOf(':');

                if (separatorIndex == -1)
                {
                    return false;
                }

                var intStringValue = s.Slice(0, separatorIndex);
                var typeStringValue = s.Slice(separatorIndex + 1).ToString();

                if (!typeStringValue.SequenceEqual("even") && !typeStringValue.SequenceEqual("odd")) 
                {
                    return false;
                }

                if (!int.TryParse(intStringValue, out int v))
                {
                    return false;
                }

                value = new OddOrEvenNumber(v);
                return true;
            }
        }

    }
}