﻿using FastCSV.Converters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV
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
            Assert.AreEqual($"value\n{byteValue}", CsvConverter.Serialize(byteValue));
            Assert.AreEqual($"value\n{shortValue}", CsvConverter.Serialize(shortValue));
            Assert.AreEqual($"value\n{intValue}", CsvConverter.Serialize(intValue));
            Assert.AreEqual($"value\n{longValue}", CsvConverter.Serialize(longValue));

            Assert.AreEqual($"value\n{sbyteValue}", CsvConverter.Serialize(sbyteValue));
            Assert.AreEqual($"value\n{ushortValue}", CsvConverter.Serialize(ushortValue));
            Assert.AreEqual($"value\n{uintValue}", CsvConverter.Serialize(uintValue));
            Assert.AreEqual($"value\n{ulongValue}", CsvConverter.Serialize(ulongValue));

            Assert.AreEqual($"value\n{halfValue}", CsvConverter.Serialize(halfValue));
            Assert.AreEqual($"value\n{floatValue}", CsvConverter.Serialize(floatValue));
            Assert.AreEqual($"value\n{doubleValue}", CsvConverter.Serialize(doubleValue));
            Assert.AreEqual($"value\n{decimalValue}", CsvConverter.Serialize(decimalValue));

            Assert.AreEqual($"value\ntrue", CsvConverter.Serialize(trueValue));
            Assert.AreEqual($"value\nfalse", CsvConverter.Serialize(falseValue));

            Assert.AreEqual($"value\n{dateTimeValue}", CsvConverter.Serialize(dateTimeValue));
            Assert.AreEqual($"value\n{dateTimeOffsetValue}", CsvConverter.Serialize(dateTimeOffsetValue));
            Assert.AreEqual($"value\n{timeSpanValue}", CsvConverter.Serialize(timeSpanValue));
            Assert.AreEqual($"value\n{versionValue}", CsvConverter.Serialize(versionValue));
            Assert.AreEqual($"value\n{ipAddressValue}", CsvConverter.Serialize(ipAddressValue));
            Assert.AreEqual($"value\n{ipEndpointValue}", CsvConverter.Serialize(ipEndpointValue));

            Assert.AreEqual($"value\n{stringValue}", CsvConverter.Serialize(stringValue));
        }

        [Test]
        public void SerializeTypeObjectTest()
        {
            Assert.AreEqual($"value\n{byteValue}", CsvConverter.Serialize(byteValue, typeof(object)));
            Assert.AreEqual($"value\n{shortValue}", CsvConverter.Serialize(shortValue, typeof(object)));
            Assert.AreEqual($"value\n{intValue}", CsvConverter.Serialize(intValue, typeof(object)));
            Assert.AreEqual($"value\n{longValue}", CsvConverter.Serialize(longValue, typeof(object)));

            Assert.AreEqual($"value\n{sbyteValue}", CsvConverter.Serialize(sbyteValue, typeof(object)));
            Assert.AreEqual($"value\n{ushortValue}", CsvConverter.Serialize(ushortValue, typeof(object)));
            Assert.AreEqual($"value\n{uintValue}", CsvConverter.Serialize(uintValue, typeof(object)));
            Assert.AreEqual($"value\n{ulongValue}", CsvConverter.Serialize(ulongValue, typeof(object)));

            Assert.AreEqual($"value\n{halfValue}", CsvConverter.Serialize(halfValue, typeof(object)));
            Assert.AreEqual($"value\n{floatValue}", CsvConverter.Serialize(floatValue, typeof(object)));
            Assert.AreEqual($"value\n{doubleValue}", CsvConverter.Serialize(doubleValue, typeof(object)));
            Assert.AreEqual($"value\n{decimalValue}", CsvConverter.Serialize(decimalValue, typeof(object)));

            Assert.AreEqual($"value\ntrue", CsvConverter.Serialize(trueValue, typeof(object)));
            Assert.AreEqual($"value\nfalse", CsvConverter.Serialize(falseValue, typeof(object)));

            Assert.AreEqual($"value\n{dateTimeValue}", CsvConverter.Serialize(dateTimeValue, typeof(object)));
            Assert.AreEqual($"value\n{dateTimeOffsetValue}", CsvConverter.Serialize(dateTimeOffsetValue, typeof(object)));
            Assert.AreEqual($"value\n{timeSpanValue}", CsvConverter.Serialize(timeSpanValue, typeof(object)));
            Assert.AreEqual($"value\n{versionValue}", CsvConverter.Serialize(versionValue, typeof(object)));
            Assert.AreEqual($"value\n{ipAddressValue}", CsvConverter.Serialize(ipAddressValue, typeof(object)));
            Assert.AreEqual($"value\n{ipEndpointValue}", CsvConverter.Serialize(ipEndpointValue, typeof(object)));
        }

        [Test]
        public void DeserializeTest()
        {
            Assert.AreEqual(byteValue, CsvConverter.Deserialize($"value\n{byteValue}", typeof(object)));
            Assert.AreEqual(shortValue, CsvConverter.Deserialize($"value\n{shortValue}", typeof(object)));
            Assert.AreEqual(intValue, CsvConverter.Deserialize($"value\n{intValue}", typeof(object)));
            Assert.AreEqual(longValue, CsvConverter.Deserialize($"value\n{longValue}", typeof(object)));

            Assert.AreEqual(sbyteValue, CsvConverter.Deserialize($"value\n{sbyteValue}", typeof(object)));
            Assert.AreEqual(ushortValue, CsvConverter.Deserialize($"value\n{ushortValue}", typeof(object)));
            Assert.AreEqual(uintValue, CsvConverter.Deserialize($"value\n{uintValue}", typeof(object)));
            Assert.AreEqual(ulongValue, CsvConverter.Deserialize($"value\n{ulongValue}", typeof(object)));

            Assert.AreEqual(floatValue, CsvConverter.Deserialize($"value\n{floatValue}", typeof(object)));
            Assert.AreEqual(doubleValue, CsvConverter.Deserialize($"value\n{doubleValue}", typeof(object)));
            Assert.AreEqual(decimalValue, CsvConverter.Deserialize($"value\n{decimalValue}", typeof(object)));

            Assert.AreEqual(true, CsvConverter.Deserialize($"value\n{trueValue}", typeof(object)));
            Assert.AreEqual(false, CsvConverter.Deserialize($"value\n{falseValue}", typeof(object)));

            Assert.AreEqual(dateTimeValue, CsvConverter.Deserialize($"value\n{dateTimeValue}", typeof(object)));
            Assert.AreEqual(dateTimeOffsetValue, CsvConverter.Deserialize($"value\n{dateTimeOffsetValue}", typeof(object)));
            Assert.AreEqual(timeSpanValue, CsvConverter.Deserialize($"value\n{timeSpanValue}", typeof(object)));
            Assert.AreEqual(versionValue, CsvConverter.Deserialize($"value\n{versionValue}", typeof(object)));
            Assert.AreEqual(ipAddressValue, CsvConverter.Deserialize($"value\n{ipAddressValue}", typeof(object)));
            Assert.AreEqual(ipEndpointValue, CsvConverter.Deserialize($"value\n{ipEndpointValue}", typeof(object)));

            Assert.AreEqual(stringValue, CsvConverter.Deserialize($"value\n{stringValue}", typeof(object)));
        }

        [Test]
        public void DeserializeHalfTest()
        {
            Half f = (Half)(float)CsvConverter.Deserialize($"value\n{halfValue}", typeof(object));       
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
            object result = CsvConverter.Deserialize("value\n34005.230211", typeof(object), options);

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