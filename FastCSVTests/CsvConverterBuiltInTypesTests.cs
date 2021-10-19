using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FastCSV;
using NUnit.Framework;

namespace FastCSV.Tests
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

            Assert.AreEqual($"value{Environment.NewLine}true", CsvConverter.Serialize(true));
            Assert.AreEqual($"value{Environment.NewLine}z", CsvConverter.Serialize('z'));

            Assert.AreEqual($"value{Environment.NewLine}2", CsvConverter.Serialize((byte)2));
            Assert.AreEqual($"value{Environment.NewLine}3", CsvConverter.Serialize((short)3));
            Assert.AreEqual($"value{Environment.NewLine}5", CsvConverter.Serialize((int)5));
            Assert.AreEqual($"value{Environment.NewLine}7", CsvConverter.Serialize((long)7));

            Assert.AreEqual($"value{Environment.NewLine}9.1", CsvConverter.Serialize((decimal)9.1));
            Assert.AreEqual($"value{Environment.NewLine}11.2", CsvConverter.Serialize((float)11.2));
            Assert.AreEqual($"value{Environment.NewLine}13.3", CsvConverter.Serialize((double)13.3));

            Assert.AreEqual($"value{Environment.NewLine}17", CsvConverter.Serialize((sbyte)17));
            Assert.AreEqual($"value{Environment.NewLine}19", CsvConverter.Serialize((ushort)19));
            Assert.AreEqual($"value{Environment.NewLine}21", CsvConverter.Serialize((uint)21));
            Assert.AreEqual($"value{Environment.NewLine}23", CsvConverter.Serialize((ulong)23));

            // Enum
            Assert.AreEqual($"value{Environment.NewLine}One", CsvConverter.Serialize(PositiveNumber.One));

            // Other types
            Assert.AreEqual($"value{Environment.NewLine}Red Apple", CsvConverter.Serialize("Red Apple"));
            Assert.AreEqual($"value{Environment.NewLine}144530730739", CsvConverter.Serialize(new BigInteger(144530730739)));
            Assert.AreEqual($"value{Environment.NewLine}91.3", CsvConverter.Serialize((Half)91.3));
            Assert.AreEqual($"value{Environment.NewLine}10/9/2021 12:00:00 a. m.", CsvConverter.Serialize(new DateTime(2021, 9, 10)));
            Assert.AreEqual($"value{Environment.NewLine}10/9/2021 12:00:00 a. m. -04:00", CsvConverter.Serialize(new DateTimeOffset(new DateTime(2021, 9, 10))));
            Assert.AreEqual($"value{Environment.NewLine}02:30:25", CsvConverter.Serialize(new TimeSpan(2, 30, 25)));
            Assert.AreEqual($"value{Environment.NewLine}127.0.0.1", CsvConverter.Serialize(new IPAddress(new byte[] { 127, 0, 0, 1 })));
            Assert.AreEqual($"value{Environment.NewLine}127.0.0.1:8080", CsvConverter.Serialize(new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8080)));
            Assert.AreEqual($"value{Environment.NewLine}1.2.345", CsvConverter.Serialize(new Version(1, 2, 345)));
            Assert.AreEqual($"value{Environment.NewLine}947e7968-13ba-4814-bd67-8243ce554fa4", CsvConverter.Serialize(new Guid("947e7968-13ba-4814-bd67-8243ce554fa4")));
            Assert.AreEqual($"value{Environment.NewLine}123", CsvConverter.Serialize(new IntPtr(123)));
            Assert.AreEqual($"value{Environment.NewLine}321", CsvConverter.Serialize(new UIntPtr(321)));
        }

        [Test]
        public void DeserializeBuiltInTypeTest()
        {
            // Primitive types
            Assert.AreEqual(false, CsvConverter.Deserialize($"value{Environment.NewLine}false", typeof(bool)));
            Assert.AreEqual('a', CsvConverter.Deserialize($"value{Environment.NewLine}a", typeof(char)));

            Assert.AreEqual((byte)2, CsvConverter.Deserialize($"value{Environment.NewLine}2", typeof(byte)));
            Assert.AreEqual((short)3, CsvConverter.Deserialize($"value{Environment.NewLine}3", typeof(short)));
            Assert.AreEqual((int)4, CsvConverter.Deserialize($"value{Environment.NewLine}4", typeof(int)));
            Assert.AreEqual((long)5, CsvConverter.Deserialize($"value{Environment.NewLine}5", typeof(long)));

            Assert.AreEqual((sbyte)6, CsvConverter.Deserialize($"value{Environment.NewLine}6", typeof(sbyte)));
            Assert.AreEqual((ushort)7, CsvConverter.Deserialize($"value{Environment.NewLine}7", typeof(ushort)));
            Assert.AreEqual((uint)8, CsvConverter.Deserialize($"value{Environment.NewLine}8", typeof(uint)));
            Assert.AreEqual((ulong)9, CsvConverter.Deserialize($"value{Environment.NewLine}9", typeof(ulong)));

            Assert.AreEqual((decimal)10.4, CsvConverter.Deserialize($"value{Environment.NewLine}10.4", typeof(decimal)));
            Assert.AreEqual((float)11.5, CsvConverter.Deserialize($"value{Environment.NewLine}11.5", typeof(float)));
            Assert.AreEqual((double)12.6, CsvConverter.Deserialize($"value{Environment.NewLine}12.6", typeof(double)));

            // Enum
            Assert.AreEqual(PositiveNumber.Two, CsvConverter.Deserialize($"value{Environment.NewLine}Two", typeof(PositiveNumber)));

            // Other types
            Assert.AreEqual("Blue house", CsvConverter.Deserialize($"value{Environment.NewLine}Blue house", typeof(string)));
            Assert.AreEqual(new BigInteger(1237894560), CsvConverter.Deserialize($"value{Environment.NewLine}1237894560", typeof(BigInteger)));
            Assert.AreEqual((Half)23.2, CsvConverter.Deserialize($"value{Environment.NewLine}23.2", typeof(Half)));
            Assert.AreEqual(new DateTime(2021, 9, 10), CsvConverter.Deserialize($"value{Environment.NewLine}10/9/2021 12:00:00 a. m.", typeof(DateTime)));
            Assert.AreEqual(new DateTimeOffset(new DateTime(2021, 9, 10)), CsvConverter.Deserialize($"value{Environment.NewLine}10/9/2021 12:00:00 a. m. -04:00", typeof(DateTimeOffset)));
            Assert.AreEqual(new TimeSpan(2, 30, 25), CsvConverter.Deserialize($"value{Environment.NewLine}02:30:25", typeof(TimeSpan)));
            Assert.AreEqual(new IPAddress(new byte[] { 127, 0, 0, 1 }), CsvConverter.Deserialize($"value{Environment.NewLine}127.0.0.1", typeof(IPAddress)));
            Assert.AreEqual(new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8080), CsvConverter.Deserialize($"value{Environment.NewLine}127.0.0.1:8080", typeof(IPEndPoint)));
            Assert.AreEqual(new Version(1, 2, 345), CsvConverter.Deserialize($"value{Environment.NewLine}1.2.345", typeof(Version)));
            Assert.AreEqual(new Guid("b5cc2c3e-1d62-433a-b1fe-bbec2fb694ac"), CsvConverter.Deserialize($"value{Environment.NewLine}b5cc2c3e-1d62-433a-b1fe-bbec2fb694ac", typeof(Guid)));
            Assert.AreEqual(new IntPtr(123), CsvConverter.Deserialize($"value{Environment.NewLine}123", typeof(IntPtr)));
            Assert.AreEqual(new UIntPtr(321), CsvConverter.Deserialize($"value{Environment.NewLine}321", typeof(UIntPtr)));
        }
    }
}
