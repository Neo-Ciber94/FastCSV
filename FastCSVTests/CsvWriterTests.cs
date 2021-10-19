using NUnit.Framework;
using System;
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

            Assert.AreEqual($"name,age{Environment.NewLine}Kenny,40{Environment.NewLine}Levi,30{Environment.NewLine}", streamReader.ReadToEnd());
        }

        [Test]
        public void WriteFileTest()
        {
            var header = CsvHeader.FromValues("Name", "Age");
            var records = new CsvRecord[]
            {
                new CsvRecord(header, new string[]{"Karl", "26"}),
                new CsvRecord(header, new string[]{"Elena", "17"})
            };

            using (var tempFile = new TempFile())
            {
                CsvWriter.WriteToFile(records, header, tempFile.FullName);

                using (var reader = new StreamReader(tempFile.FullName))
                {
                    string data = reader.ReadToEnd();
                    Assert.AreEqual(
                        $"Name,Age{Environment.NewLine}" +
                        $"Karl,26{Environment.NewLine}" +
                        $"Elena,17{Environment.NewLine}", data);
                }
            }
        }

        [Test]
        public void WriteFileTest1()
        {
            var list = new Person[]
            {
                new Person{ Name="Karl", Age = 26},
                new Person{ Name="Elena", Age = 17 }
            };

            using (var tempFile = new TempFile())
            {
                CsvWriter.WriteValuesToFile(list, tempFile.FullName);

                using (var reader = new StreamReader(tempFile.FullName))
                {
                    string data = reader.ReadToEnd();
                    Assert.AreEqual(
                        $"Name,Age{Environment.NewLine}" +
                        $"Karl,26{Environment.NewLine}" +
                        $"Elena,17{Environment.NewLine}", data);
                }
            }
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

            Assert.AreEqual($"name,age{Environment.NewLine}Kenny,40{Environment.NewLine}Levi,Ackerman,30{Environment.NewLine}{Environment.NewLine}", streamReader.ReadToEnd());
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

            Assert.AreEqual($"name,age{Environment.NewLine}Kenny,40{Environment.NewLine}Levi,30{Environment.NewLine}", streamReader.ReadToEnd());
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

            Assert.AreEqual($"name , age{Environment.NewLine} Kenny,40{Environment.NewLine} Levi ,30{Environment.NewLine}", streamReader.ReadToEnd());
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

            Assert.AreEqual($"name,age{Environment.NewLine}Kenny,40{Environment.NewLine}Levi,30{Environment.NewLine}", streamReader.ReadToEnd());
        }

        [Test()]
        public void WriteWithTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            writer.WriteValue(new { Name = "Kenny", Age = "40" });
            writer.WriteValue(new { Name = "Levi", Age = "30" });

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual($"Kenny,40{Environment.NewLine}Levi,30{Environment.NewLine}", streamReader.ReadToEnd());
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

            Assert.AreEqual($"name,age{Environment.NewLine}Kenny,40{Environment.NewLine}Levi,30{Environment.NewLine}", streamReader.ReadToEnd());
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

            Assert.AreEqual($"name,age{Environment.NewLine}Kenny,40{Environment.NewLine}Levi,30{Environment.NewLine}", streamReader.ReadToEnd());
        }

        [Test()]
        public async Task WriteWithAsyncTest()
        {
            using var memory = new MemoryStream();
            using var writer = new CsvWriter(new StreamWriter(memory));

            await writer.WriteValueAsync(new { Name = "Kenny", Age = "40" });
            await writer.WriteValueAsync(new { Name = "Levi", Age = "30" });

            memory.Position = 0;
            using var streamReader = new StreamReader(memory);

            Assert.AreEqual($"Kenny,40{Environment.NewLine}Levi,30{Environment.NewLine}", streamReader.ReadToEnd());
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

        class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}