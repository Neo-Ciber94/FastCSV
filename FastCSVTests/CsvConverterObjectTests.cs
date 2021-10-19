using FastCSV.Converters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;

namespace FastCSV.Tests
{
    [TestFixture]
    class CsvConverterObjectTests
    {
        private readonly object byteValue = (byte)230;
        private readonly object shortValue = (short)31911;
        private readonly object intValue = 12;
        private readonly object longValue = -3_432_200_100_999_432;

        private readonly object sbyteValue = (sbyte)-120;
        private readonly object ushortValue = (ushort)44911;
        private readonly object uintValue = (uint)3_500_342_200;
        private readonly object ulongValue = (ulong)9_432_200_100_999_432;

        private readonly object halfValue = (Half)323.23;
        private readonly object floatValue = 30.45f;
        private readonly object doubleValue = 45600000000000000000000000000000000000000000000000d;
        private readonly object decimalValue = 2500m;

        private readonly object trueValue = true;
        private readonly object falseValue = false;

        private readonly object dateTimeValue = new DateTime(2020, 5, 4, 10, 30, 20);
        private readonly object dateTimeOffsetValue = new DateTime(2020, 5, 4, 10, 30, 20);
        private readonly object timeSpanValue = new TimeSpan(50, 40, 30, 20, 10);
        private readonly object versionValue = new Version(1, 5, 234);
        private readonly object ipAddressValue = new IPAddress(new byte[] { 127, 0, 0, 1});
        private readonly object ipEndpointValue = new IPEndPoint(127001, 8080);
        private readonly object stringValue = "This is so cool!";

        [Test]
        public void SerializeTest()
        {
            Assert.AreEqual($"value{System.Environment.NewLine}{byteValue}", CsvConverter.Serialize(byteValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{shortValue}", CsvConverter.Serialize(shortValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{intValue}", CsvConverter.Serialize(intValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{longValue}", CsvConverter.Serialize(longValue));

            Assert.AreEqual($"value{System.Environment.NewLine}{sbyteValue}", CsvConverter.Serialize(sbyteValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{ushortValue}", CsvConverter.Serialize(ushortValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{uintValue}", CsvConverter.Serialize(uintValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{ulongValue}", CsvConverter.Serialize(ulongValue));

            Assert.AreEqual($"value{System.Environment.NewLine}{halfValue}", CsvConverter.Serialize(halfValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{floatValue}", CsvConverter.Serialize(floatValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{doubleValue}", CsvConverter.Serialize(doubleValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{decimalValue}", CsvConverter.Serialize(decimalValue));

            Assert.AreEqual($"value{System.Environment.NewLine}true", CsvConverter.Serialize(trueValue));
            Assert.AreEqual($"value{System.Environment.NewLine}false", CsvConverter.Serialize(falseValue));

            Assert.AreEqual($"value{System.Environment.NewLine}{dateTimeValue}", CsvConverter.Serialize(dateTimeValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{dateTimeOffsetValue}", CsvConverter.Serialize(dateTimeOffsetValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{timeSpanValue}", CsvConverter.Serialize(timeSpanValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{versionValue}", CsvConverter.Serialize(versionValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{ipAddressValue}", CsvConverter.Serialize(ipAddressValue));
            Assert.AreEqual($"value{System.Environment.NewLine}{ipEndpointValue}", CsvConverter.Serialize(ipEndpointValue));

            Assert.AreEqual($"value{System.Environment.NewLine}{stringValue}", CsvConverter.Serialize(stringValue));
        }

        [Test]
        public void SerializeTypeObjectTest()
        {
            Assert.AreEqual($"value{System.Environment.NewLine}{byteValue}", CsvConverter.Serialize(byteValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{shortValue}", CsvConverter.Serialize(shortValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{intValue}", CsvConverter.Serialize(intValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{longValue}", CsvConverter.Serialize(longValue, typeof(object)));

            Assert.AreEqual($"value{System.Environment.NewLine}{sbyteValue}", CsvConverter.Serialize(sbyteValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{ushortValue}", CsvConverter.Serialize(ushortValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{uintValue}", CsvConverter.Serialize(uintValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{ulongValue}", CsvConverter.Serialize(ulongValue, typeof(object)));

            Assert.AreEqual($"value{System.Environment.NewLine}{halfValue}", CsvConverter.Serialize(halfValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{floatValue}", CsvConverter.Serialize(floatValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{doubleValue}", CsvConverter.Serialize(doubleValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{decimalValue}", CsvConverter.Serialize(decimalValue, typeof(object)));

            Assert.AreEqual($"value{System.Environment.NewLine}true", CsvConverter.Serialize(trueValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}false", CsvConverter.Serialize(falseValue, typeof(object)));

            Assert.AreEqual($"value{System.Environment.NewLine}{dateTimeValue}", CsvConverter.Serialize(dateTimeValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{dateTimeOffsetValue}", CsvConverter.Serialize(dateTimeOffsetValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{timeSpanValue}", CsvConverter.Serialize(timeSpanValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{versionValue}", CsvConverter.Serialize(versionValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{ipAddressValue}", CsvConverter.Serialize(ipAddressValue, typeof(object)));
            Assert.AreEqual($"value{System.Environment.NewLine}{ipEndpointValue}", CsvConverter.Serialize(ipEndpointValue, typeof(object)));
        }

        [Test]
        public void DeserializeTest()
        {
            Assert.AreEqual(byteValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{byteValue}", typeof(object)));
            Assert.AreEqual(shortValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{shortValue}", typeof(object)));
            Assert.AreEqual(intValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{intValue}", typeof(object)));
            Assert.AreEqual(longValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{longValue}", typeof(object)));

            Assert.AreEqual(sbyteValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{sbyteValue}", typeof(object)));
            Assert.AreEqual(ushortValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{ushortValue}", typeof(object)));
            Assert.AreEqual(uintValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{uintValue}", typeof(object)));
            Assert.AreEqual(ulongValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{ulongValue}", typeof(object)));

            Assert.AreEqual(floatValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{floatValue}", typeof(object)));
            Assert.AreEqual(doubleValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{doubleValue}", typeof(object)));
            Assert.AreEqual(decimalValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{decimalValue}", typeof(object)));

            Assert.AreEqual(true, CsvConverter.Deserialize($"value{System.Environment.NewLine}{trueValue}", typeof(object)));
            Assert.AreEqual(false, CsvConverter.Deserialize($"value{System.Environment.NewLine}{falseValue}", typeof(object)));

            Assert.AreEqual(dateTimeValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{dateTimeValue}", typeof(object)));
            Assert.AreEqual(dateTimeOffsetValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{dateTimeOffsetValue}", typeof(object)));
            Assert.AreEqual(timeSpanValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{timeSpanValue}", typeof(object)));
            Assert.AreEqual(versionValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{versionValue}", typeof(object)));
            Assert.AreEqual(ipAddressValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{ipAddressValue}", typeof(object)));
            Assert.AreEqual(ipEndpointValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{ipEndpointValue}", typeof(object)));

            Assert.AreEqual(stringValue, CsvConverter.Deserialize($"value{System.Environment.NewLine}{stringValue}", typeof(object)));
        }

        [Test]
        public void DeserializeHalfTest()
        {
            Half f = (Half)(float)CsvConverter.Deserialize($"value{System.Environment.NewLine}{halfValue}", typeof(object));       
            Assert.AreEqual(halfValue, f);
        }

        [Test]
        public void DeserializeWithTypeGuesserTest()
        {
            var options = new CsvConverterOptions
            {
                Converters = new List<ICsvValueConverter>() { DecimalConverter.Default },
                TypeGuessers = new List<ITypeGuesser>() { DecimalConverter.Default }
            };

            decimal value = 34005.230211m;
            object result = CsvConverter.Deserialize($"value{System.Environment.NewLine}34005.230211", typeof(object), options);

            Assert.AreEqual(typeof(decimal), result.GetType());
            Assert.AreEqual(value, result);
        }

        class DecimalConverter : ITypeGuesser, ICsvValueConverter<decimal>
        {
            public static DecimalConverter Default { get; } = new DecimalConverter();

            public Type GetTypeFromString(string s)
            {
                if (decimal.TryParse(s, out _))
                {
                    return typeof(decimal);
                }

                return null;
            }

            public bool TryDeserialize(out decimal value, ref CsvDeserializeState state)
            {
                return decimal.TryParse(state.Read(), out value);
            }

            public bool TrySerialize(decimal value, ref CsvSerializeState state)
            {
                state.Write(value.ToString());
                return true;
            }
        }
    }
}
