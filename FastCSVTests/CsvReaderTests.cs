#nullable enable

using NUnit.Framework;
using FastCSV;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace FastCSV.Tests
{
    [TestFixture()]
    public class CsvReaderTests
    {
        private static MemoryStream ToStream(string data)
        {
            var memory = new MemoryStream(data.Length);
            using (var writer = new StreamWriter(memory, leaveOpen: true))
            {
                writer.Write(data);
            }

            memory.Position = 0;
            return memory;
        }

        [Test()]
        public unsafe void CsvReaderTest()
        {
            using var csv = ToStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));

            Assert.AreEqual(CsvFormat.Default, reader.Format);
            Assert.IsTrue(reader.HasHeader);
            Assert.IsFalse(reader.Done);
        }

        [Test()]
        public void CsvReaderTest1()
        {
            using var csv = ToStream(
                    "name,age\n" +
                    "Homer,35\n" +
                    "Marge,28\n");

            var format = new CsvFormat('\t', '\"');
            using var reader = new CsvReader(new StreamReader(csv), format);

            Assert.AreEqual(new CsvFormat('\t', '\"'), reader.Format);
            Assert.IsTrue(reader.HasHeader);
            Assert.IsFalse(reader.Done);
        }

        [Test()]
        public void CsvReaderTest2()
        {
            using var csv = ToStream(
                    "name,age\n" +
                    "Homer,35\n" +
                    "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false);

            Assert.AreEqual(CsvFormat.Default, reader.Format);
            Assert.IsFalse(reader.HasHeader);
            Assert.IsFalse(reader.Done);
        }

        [Test()]
        public void FromStreamTest()
        {
            using var csv = ToStream(
                    "name,age\n" +
                    "Homer,35\n" +
                    "Marge,28\n");

            using var reader = CsvReader.FromStream(csv);

            Assert.AreEqual(CsvFormat.Default, reader.Format);
            Assert.IsTrue(reader.HasHeader);
            Assert.IsFalse(reader.Done);
        }

        [Test()]
        public void ReadTest()
        {
            using var csv = ToStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));
            Assert.IsFalse(reader.Done);
            Assert.AreEqual(0, reader.RecordNumber);

            Assert.AreEqual("name,age", reader.Header!.ToString());
            Assert.AreEqual("Homer,35", reader.Read()!.ToString());
            Assert.AreEqual("Marge,28", reader.Read()!.ToString());
            Assert.IsNull(reader.Read());
            Assert.IsTrue(reader.Done);
            Assert.AreEqual(2, reader.RecordNumber);
        }

        [Test()]
        public void ReadEmptyTest1()
        {
            using var csv = ToStream("");

            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false);
            Assert.IsNull(reader.Read());
        }


        [Test()]
        public void ReadEmptyTest2()
        {
            using var csv = ToStream("");

            var format = CsvFormat.Default.WithIgnoreWhitespace(false);
            using var reader = new CsvReader(new StreamReader(csv), format, hasHeader: false);
            Assert.AreEqual("", reader.Read()!.ToString());
        }

        [Test()]
        public void ReadBlackTest1()
        {
            using var csv = ToStream(" ");

            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false);
            Assert.IsNull(reader.Read());
        }

        [Test()]
        public void ReadBlackTest2()
        {
            using var csv = ToStream(" ");

            var format = CsvFormat.Default.WithIgnoreWhitespace(false);
            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false, format: format);
            Assert.AreEqual(" ", reader.Read()!.ToString());
        }

        [Test()]
        public async Task ReadAsyncTest()
        {
            using var csv = ToStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));
            Assert.IsFalse(reader.Done);

            Assert.AreEqual("name,age", reader.Header!.ToString());
            Assert.AreEqual("Homer,35", (await reader.ReadAsync())!.ToString());
            Assert.AreEqual("Marge,28", (await reader.ReadAsync())!.ToString());

            CsvRecord? record = await reader.ReadAsync();
            Assert.IsNull(record);
            Assert.IsTrue(reader.Done);
        }

        [Test()]
        public void ReadAllTest()
        {
            using var csv = ToStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));
            var enumerator = reader.ReadAll().GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Homer,35", enumerator.Current.ToString());

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Marge,28", enumerator.Current.ToString());

            Assert.IsFalse(enumerator.MoveNext());
            Assert.IsTrue(reader.Done);
        }

        [Test()]
        public async Task ReadAllAsyncTest()
        {
            using var csv = ToStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));
            var enumerator = reader.ReadAllAsync().GetAsyncEnumerator();

            Assert.IsTrue(await enumerator.MoveNextAsync());
            Assert.AreEqual("Homer,35", enumerator.Current!.ToString());

            Assert.IsTrue(await enumerator.MoveNextAsync());
            Assert.AreEqual("Marge,28", enumerator.Current.ToString());

            Assert.IsFalse(await enumerator.MoveNextAsync());
            Assert.IsTrue(reader.Done);
        }

        [Test()]
        public void ResetTest()
        {
            using var csv = ToStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));

            // Force to consume all the records
            foreach (var _ in reader.ReadAll()) { }

            reader.Reset();

            var enumerator = reader.ReadAll().GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Homer,35", enumerator.Current.ToString());

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Marge,28", enumerator.Current.ToString());

            Assert.IsFalse(enumerator.MoveNext());
            Assert.IsTrue(reader.Done);
        }

        [Test()]
        public void TryResetTest()
        {
            using var csv = ToStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));

            // Force to consume all the records
           foreach(var _ in reader.ReadAll()) { }

            Assert.IsTrue(reader.TryReset());

            var enumerator = reader.ReadAll().GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Homer,35", enumerator.Current.ToString());

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Marge,28", enumerator.Current.ToString());

            Assert.IsFalse(enumerator.MoveNext());
            Assert.IsTrue(reader.Done);
        }

        [Test()]
        public void CloseTest()
        {
            using var csv = new MemoryStream();
            var reader = CsvReader.FromStream(csv);
            reader.Close();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                var _ = reader.Read();
            });
        }

        [Test()]
        public void DisposeTest()
        {
            using var csv = new MemoryStream();
            var reader = CsvReader.FromStream(csv);
            reader.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                var _ = reader.Read();
            });
        }
    }
}