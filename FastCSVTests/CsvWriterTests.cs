using NUnit.Framework;
using FastCSV;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FastCSV.Tests
{
    [TestFixture()]
    public class CsvWriterTests
    {
        [Test()]
        public void CsvWriterTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            Assert.AreEqual(CsvFormat.Default, writer.Format);
            Assert.IsFalse(writer.IsFlexible);
        }

        [Test()]
        public void CsvWriterTest2()
        {
            var format = new CsvFormat('\t', '\"');
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory), format);

            Assert.AreEqual(new CsvFormat('\t', '\"'), writer.Format);
            Assert.IsFalse(writer.IsFlexible);
        }

        [Test()]
        public void CsvWriterTest3()
        {
            var format = new CsvFormat('\t', '\"');
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory), format, flexible: true);

            Assert.AreEqual(new CsvFormat('\t', '\"'), writer.Format);
            Assert.IsTrue(writer.IsFlexible);
        }

        [Test()]
        public void WriteTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            writer.Write("name", "age");
            writer.Write("Kenny", 40);
            writer.Write("Levi", 30);

            Assert.Throws<ArgumentException>(() =>
            {
                writer.Write();
            });

            Assert.Throws<ArgumentException>(() =>
            {
                writer.Write("Mikasa", "Ackerman", 16);
            });

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual("name,age\r\nKenny,40\r\nLevi,30\r\n", streamReader.ReadToEnd());
        }

        [Test()]
        public void WriteFlexibleTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory), flexible: true);

            writer.Write("name", "age");
            writer.Write("Kenny", 40);
            writer.Write("Levi", "Ackerman", 30);
            writer.Write(); // Flexible allow empty records

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual("name,age\r\nKenny,40\r\nLevi,Ackerman,30\r\n\r\n", streamReader.ReadToEnd());
        }

        [Test()]
        public void WriteWithWhitespaceTest1()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            writer.Write("name ", " age");
            writer.Write(" Kenny", 40);
            writer.Write(" Levi ", 30);

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual("name,age\r\nKenny,40\r\nLevi,30\r\n", streamReader.ReadToEnd());
        }

        [Test()]
        public void WriteWithWhitespaceTest2()
        {
            using var memory = new MemoryStream();
            var format = CsvFormat.Default.WithIgnoreWhitespace(false);
            using var writer = new CsvWriter(new StreamWriter(memory), format);

            writer.Write("name ", " age");
            writer.Write(" Kenny", 40);
            writer.Write(" Levi ", 30);

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual("name , age\r\n Kenny,40\r\n Levi ,30\r\n", streamReader.ReadToEnd());
        }

        [Test()]
        public void WriteAllTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            writer.WriteAll(new string[] { "name", "age" });
            writer.WriteAll(new string[] { "Kenny", "40" });
            writer.WriteAll(new string[] { "Levi", "30" });

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual("name,age\r\nKenny,40\r\nLevi,30\r\n", streamReader.ReadToEnd());
        }

        [Test()]
        public void WriteWithTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            writer.WriteWith(new { Name = "Kenny", Age = "40" });
            writer.WriteWith(new { Name = "Levi", Age = "30" });

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual("Kenny,40\r\nLevi,30\r\n", streamReader.ReadToEnd());
        }

        [Test()]
        public async Task WriteAsyncTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            await writer.WriteAsync("name", "age");
            await writer.WriteAsync("Kenny", 40);
            await writer.WriteAsync("Levi", 30);

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual("name,age\r\nKenny,40\r\nLevi,30\r\n", streamReader.ReadToEnd());
        }

        [Test()]
        public async Task WriteAllAsyncTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            await writer.WriteAllAsync(new string[] { "name", "age" });
            await writer.WriteAllAsync(new string[] { "Kenny", "40" });
            await writer.WriteAllAsync(new string[] { "Levi", "30" });

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual("name,age\r\nKenny,40\r\nLevi,30\r\n", streamReader.ReadToEnd());
        }

        [Test()]
        public async Task WriteWithAsyncTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            await writer.WriteWithAsync(new { Name = "Kenny", Age = "40" });
            await writer.WriteWithAsync(new { Name = "Levi", Age = "30" });

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual("Kenny,40\r\nLevi,30\r\n", streamReader.ReadToEnd());
        }

        [Test()]
        public void CloseTest()
        {
            using var memory = new MemoryStream();
            var writer = new CsvWriter(new StreamWriter(memory));
            writer.Close();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                writer.Write("Eren", 16);
            });
        }

        [Test()]
        public void DisposeTest()
        {
            using var memory = new MemoryStream();
            var writer = new CsvWriter(new StreamWriter(memory));
            writer.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                writer.Write("Eren", 16);
            });
        }
    }
}