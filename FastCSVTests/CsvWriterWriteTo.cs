using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvWriterWriteTo
    {
        private static readonly string CsvData;

        static CsvWriterWriteTo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("name,price,color");
            sb.AppendLine("keyboard,2000,black");
            sb.AppendLine("mouse,500,white");
            sb.AppendLine("monitor,24500.99,gray");

            CsvData = sb.ToString();
        }

        [Test]
        public void WriteEnumerableToStreamTest()
        {
            using var memoryStream = new MemoryStream(200);
            var products = new List<Product>
            {
                new ("Keyboard", 2000m, "black"),
                new ("Mouse", 500m, "white"),
                new ("Monitor", 24500.99m, "gray"),
            };

            CsvWriter.WriteToStream(products, memoryStream, new CsvFormat(delimiter: ';'));
            memoryStream.Position = 0;

            string data = ReadAllStream(memoryStream);
            string[] lines = data.Split("\r\n");

            Assert.AreEqual("Name;Price;Color", lines[0]);
            Assert.AreEqual("Keyboard;2000;black", lines[1]);
            Assert.AreEqual("Mouse;500;white", lines[2]);
            Assert.AreEqual("Monitor;24500.99;gray", lines[3]);
        }

        [Test]
        public void WriteRecordsToStreamTest()
        {
            using var memoryStream = new MemoryStream(200);
            var header = CsvHeader.FromValues("Name", "Price", "Color");

            var products = new List<CsvRecord>
            {
                new CsvRecord(header, new []{ "Keyboard", "2000", "black" }),
                new CsvRecord(header, new []{ "Mouse", "500", "white" }),
                new CsvRecord(header, new []{ "Monitor", "24500.99", "gray" }),
            };

            CsvWriter.WriteToStream(products, header, memoryStream);
            memoryStream.Position = 0;

            string data = ReadAllStream(memoryStream);
            string[] lines = data.Split("\r\n");

            Assert.AreEqual("Name,Price,Color", lines[0]);
            Assert.AreEqual("Keyboard,2000,black", lines[1]);
            Assert.AreEqual("Mouse,500,white", lines[2]);
            Assert.AreEqual("Monitor,24500.99,gray", lines[3]);
        }

        record Product(string Name, decimal Price, string Color);

        public static string ReadAllStream(Stream stream)
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            string result = reader.ReadToEnd();
            return result;
        }
    }
}
