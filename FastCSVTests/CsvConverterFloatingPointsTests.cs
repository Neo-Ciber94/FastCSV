using NUnit.Framework;

namespace FastCSV
{
    [TestFixture]
    public class CsvConverterFloatingPointsTests
    {
        [Test]
        public void SerializeAndDeserializeFloatTest()
        {
            float value = 12.90222222f;
            var serialize = CsvConverter.Serialize(value);
            Assert.AreEqual($"value{System.Environment.NewLine}{value}", serialize);

            var deserialize = CsvConverter.Deserialize<float>(serialize);
            Assert.AreEqual(value, deserialize);
        }

        [Test]
        public void SerializeAndDeserializeDoubleTest()
        {
            double value = 3120000000000.3333d;
            var serialize = CsvConverter.Serialize(value);
            Assert.AreEqual($"value{System.Environment.NewLine}{value}", serialize);

            var deserialize = CsvConverter.Deserialize<double>(serialize);
            Assert.AreEqual(value, deserialize);
        }
    }
}
