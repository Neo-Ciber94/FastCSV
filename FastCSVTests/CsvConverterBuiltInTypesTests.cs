using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FastCSV;
using NUnit.Framework;

namespace FastCSVTests
{
    [TestFixture]
    public class CsvConverterBuiltInTypesTests
    {
        enum PositiveNumber
        {
            One, Two, Three
        }

        [Test]
        public void SerializeBuiltInTypeTest()
        {
            // Primitives and decimal

            Assert.AreEqual($"value\ntrue", CsvConverter.Serialize(true));
            Assert.AreEqual("value\nz", CsvConverter.Serialize('z'));

            Assert.AreEqual("value\n2", CsvConverter.Serialize((byte)2));
            Assert.AreEqual("value\n3", CsvConverter.Serialize((short)3));
            Assert.AreEqual("value\n5", CsvConverter.Serialize((int)5));
            Assert.AreEqual("value\n7", CsvConverter.Serialize((long)7));

            Assert.AreEqual("value\n9.1", CsvConverter.Serialize((decimal)9.1));
            Assert.AreEqual("value\n11.2", CsvConverter.Serialize((float)11.2));
            Assert.AreEqual("value\n13.3", CsvConverter.Serialize((double)13.3));

            Assert.AreEqual("value\n17", CsvConverter.Serialize((sbyte)17));
            Assert.AreEqual("value\n19", CsvConverter.Serialize((ushort)19));
            Assert.AreEqual("value\n21", CsvConverter.Serialize((uint)21));
            Assert.AreEqual("value\n23", CsvConverter.Serialize((ulong)23));

            // Enum
            Assert.AreEqual("value\nOne", CsvConverter.Serialize(PositiveNumber.One));

            // Other types
            Assert.AreEqual("value\nRed Apple", CsvConverter.Serialize("Red Apple"));
            Assert.AreEqual("value\n144530730739", CsvConverter.Serialize(new BigInteger(144530730739)));
            Assert.AreEqual("value\n91.3", CsvConverter.Serialize((Half)91.3));
            Assert.AreEqual("value\n10/9/2021 12:00:00 a. m.", CsvConverter.Serialize(new DateTime(2021, 9, 10)));
            Assert.AreEqual("value\n10/9/2021 12:00:00 a. m. -04:00", CsvConverter.Serialize(new DateTimeOffset(new DateTime(2021, 9, 10))));
            Assert.AreEqual("value\n02:30:25", CsvConverter.Serialize(new TimeSpan(2, 30, 25)));
            Assert.AreEqual("value\n127.0.0.1", CsvConverter.Serialize(new IPAddress(new byte[] { 127, 0, 0, 1 })));
            Assert.AreEqual("value\n1.2.345", CsvConverter.Serialize(new Version(1, 2, 345)));
            Assert.AreEqual("value\n947e7968-13ba-4814-bd67-8243ce554fa4", CsvConverter.Serialize(new Guid("947e7968-13ba-4814-bd67-8243ce554fa4")));
            Assert.AreEqual("value\n123", CsvConverter.Serialize(new IntPtr(123)));
            Assert.AreEqual("value\n321", CsvConverter.Serialize(new UIntPtr(321)));
        }

        [Test]
        public void DeserializeBuiltInTypeTest()
        {
            // Primitive types
            Assert.AreEqual(false, CsvConverter.Deserialize("value\nfalse", typeof(bool)));
            Assert.AreEqual('a', CsvConverter.Deserialize("value\na", typeof(char)));

            Assert.AreEqual((byte)2, CsvConverter.Deserialize("value\n2", typeof(byte)));
            Assert.AreEqual((short)3, CsvConverter.Deserialize("value\n3", typeof(short)));
            Assert.AreEqual((int)4, CsvConverter.Deserialize("value\n4", typeof(int)));
            Assert.AreEqual((long)5, CsvConverter.Deserialize("value\n5", typeof(long)));

            Assert.AreEqual((sbyte)6, CsvConverter.Deserialize("value\n6", typeof(sbyte)));
            Assert.AreEqual((ushort)7, CsvConverter.Deserialize("value\n7", typeof(ushort)));
            Assert.AreEqual((uint)8, CsvConverter.Deserialize("value\n8", typeof(uint)));
            Assert.AreEqual((ulong)9, CsvConverter.Deserialize("value\n9", typeof(ulong)));

            Assert.AreEqual((decimal)10.4, CsvConverter.Deserialize("value\n10.4", typeof(decimal)));
            Assert.AreEqual((float)11.5, CsvConverter.Deserialize("value\n11.5", typeof(float)));
            Assert.AreEqual((double)12.6, CsvConverter.Deserialize("value\n12.6", typeof(double)));

            // Enum
            Assert.AreEqual(PositiveNumber.Two, CsvConverter.Deserialize("value\nTwo", typeof(PositiveNumber)));

            // Other types
            Assert.AreEqual("Blue house", CsvConverter.Deserialize("value\nBlue house", typeof(string)));
            Assert.AreEqual(new BigInteger(1237894560), CsvConverter.Deserialize("value\n1237894560", typeof(BigInteger)));
            Assert.AreEqual((Half)23.2, CsvConverter.Deserialize("value\n23.2", typeof(Half)));
            Assert.AreEqual(new DateTime(2021, 9, 10), CsvConverter.Deserialize("value\n10/9/2021 12:00:00 a. m.", typeof(DateTime)));
            Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 9, 10)), CsvConverter.Deserialize("value\n10/9/2021 12:00:00 a. m. -04:00", typeof(DateTimeOffset)));
            Assert.AreEqual(new TimeSpan(2, 30, 25), CsvConverter.Deserialize("value\n02:30:25", typeof(TimeSpan)));
            Assert.AreEqual(new IPAddress(new byte[] { 127, 0, 0, 1 }), CsvConverter.Deserialize("value\n127.0.0.1", typeof(IPAddress)));
            Assert.AreEqual(new Version(1, 2, 345), CsvConverter.Deserialize("value\n1.2.345", typeof(Version)));
            Assert.AreEqual(new Guid("b5cc2c3e-1d62-433a-b1fe-bbec2fb694ac"), CsvConverter.Deserialize("value\nb5cc2c3e-1d62-433a-b1fe-bbec2fb694ac", typeof(Guid)));
            Assert.AreEqual(new IntPtr(123), CsvConverter.Deserialize("value\n123", typeof(IntPtr)));
            Assert.AreEqual(new UIntPtr(321), CsvConverter.Deserialize("value\n321", typeof(UIntPtr)));
        }
    }
}
