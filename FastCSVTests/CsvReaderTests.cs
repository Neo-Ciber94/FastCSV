#nullable enable

using NUnit.Framework;
using FastCSV;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using FastCSV.Utils;

namespace FastCSV.Tests
{
    [TestFixture()]
    public class CsvReaderTests
    {
        [Test()]
        public unsafe void CsvReaderTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));

            Assert.AreEqual(CsvFormat.Default, reader.Format);
            Assert.IsTrue(reader.HasHeader);
            Assert.IsFalse(reader.IsDone);
        }

        [Test()]
        public void CsvReaderTest1()
        {
            using var csv = StreamHelper.ToMemoryStream(
                    "name,age\n" +
                    "Homer,35\n" +
                    "Marge,28\n");

            var format = new CsvFormat('\t', '\"');
            using var reader = new CsvReader(new StreamReader(csv), format);

            Assert.AreEqual(new CsvFormat('\t', '\"'), reader.Format);
            Assert.IsTrue(reader.HasHeader);
            Assert.IsFalse(reader.IsDone);
        }

        [Test()]
        public void CsvReaderTest2()
        {
            using var csv = StreamHelper.ToMemoryStream(
                    "name,age\n" +
                    "Homer,35\n" +
                    "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false);

            Assert.AreEqual(CsvFormat.Default, reader.Format);
            Assert.IsFalse(reader.HasHeader);
            Assert.IsFalse(reader.IsDone);
        }

        [Test()]
        public void FromStreamTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                    "name,age\n" +
                    "Homer,35\n" +
                    "Marge,28\n");

            using var reader = CsvReader.FromStream(csv);

            Assert.AreEqual(CsvFormat.Default, reader.Format);
            Assert.IsTrue(reader.HasHeader);
            Assert.IsFalse(reader.IsDone);
        }

        [Test()]
        public void ReadTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));
            Assert.IsFalse(reader.IsDone);
            Assert.AreEqual(0, reader.RecordNumber);

            Assert.AreEqual("name,age", reader.Header!.ToString());
            Assert.AreEqual("Homer,35", reader.Read()!.ToString());
            Assert.AreEqual("Marge,28", reader.Read()!.ToString());
            Assert.IsNull(reader.Read());
            Assert.IsTrue(reader.IsDone);
            Assert.AreEqual(2, reader.RecordNumber);
        }

        [Test()]
        public void ReadEmptyTest1()
        {
            using var csv = StreamHelper.ToMemoryStream("");

            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false);
            Assert.IsNull(reader.Read());
        }

        [Test()]
        public void ReadEmptyTest2()
        {
            using var csv = StreamHelper.ToMemoryStream("");

            var format = CsvFormat.Default.WithIgnoreWhitespace(false);
            using var reader = new CsvReader(new StreamReader(csv), format, hasHeader: false);
            Assert.AreEqual("", reader.Read()!.ToString());
        }

        [Test()]
        public void ReadBlackTest1()
        {
            using var csv = StreamHelper.ToMemoryStream(" ");

            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false);
            Assert.IsNull(reader.Read());
        }

        [Test()]
        public void ReadBlackTest2()
        {
            using var csv = StreamHelper.ToMemoryStream(" ");

            var format = CsvFormat.Default.WithIgnoreWhitespace(false);
            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false, format: format);
            Assert.AreEqual(" ", reader.Read()!.ToString());
        }

        [Test()]
        public void ReadRecordWithWhiteSpaceTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "Name,Age\n" +
                "Homer , 35\n" +
                " Marge,28\n");

            using var reader = CsvReader.FromStream(csv, CsvFormat.Default.WithIgnoreWhitespace(false));

            Assert.AreEqual("Homer , 35", reader.Read()!.ToString());
            Assert.AreEqual(" Marge,28", reader.Read()!.ToString());
        }

        [Test()]
        public void ReadWithUnclosedQuoteTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "Name,Age\n" +
                "Mario \"The plumber, 20\n" +
                "Luigi, 19\n");

            using var reader = CsvReader.FromStream(csv);

            Assert.Throws<CsvFormatException>(() =>
            {
                var _ = reader.Read();
            });
        }

        [Test()]
        public void ReadWithQuoteAlwaysTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "Name,Age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            var reader = CsvReader.FromStream(csv, CsvFormat.Default.WithStyle(QuoteStyle.Always));

            Assert.AreEqual("\"Homer\",\"35\"", reader.Read()!.ToString());
            Assert.AreEqual("\"Marge\",\"28\"", reader.Read()!.ToString());
        }

        [Test()]
        public void ReadWithQuoteNeverTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "Name,Age\n" +
                "Frida \"The Painter\", 35\n" +
                "Pagannini \"The violinist\",28\n");

            var reader = CsvReader.FromStream(csv, CsvFormat.Default.WithStyle(QuoteStyle.Never));
            Assert.AreEqual("Frida The Painter,35", reader.Read()!.ToString());
            Assert.AreEqual("Pagannini The violinist,28", reader.Read()!.ToString());
        }

        [Test()]
        public async Task ReadAsyncTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));
            Assert.IsFalse(reader.IsDone);

            Assert.AreEqual("name,age", reader.Header!.ToString());
            Assert.AreEqual("Homer,35", (await reader.ReadAsync())!.ToString());
            Assert.AreEqual("Marge,28", (await reader.ReadAsync())!.ToString());

            CsvRecord? record = await reader.ReadAsync();
            Assert.IsNull(record);
            Assert.IsTrue(reader.IsDone);
        }

        [Test()]
        public void ReadAllTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
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
            Assert.IsTrue(reader.IsDone);
        }

        [Test()]
        public async Task ReadAllAsyncTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
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
            Assert.IsTrue(reader.IsDone);
        }

        [Test]
        public void ReasAsTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "Name,Age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = CsvReader.FromStream(csv);

            Person p1 = reader.ReadAs<Person>().Value;
            Assert.AreEqual("Homer", p1.Name);
            Assert.AreEqual(35, p1.Age);

            Person p2 = reader.ReadAs<Person>().Value;
            Assert.AreEqual("Marge", p2.Name);
            Assert.AreEqual(28, p2.Age);

            Assert.IsFalse(reader.ReadAs<Person>().HasValue);
        }

        [Test]
        public void ReasAllAsTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "Name,Age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = CsvReader.FromStream(csv);

            var persons = reader.ReadAllAs<Person>().ToList();

            Assert.AreEqual("Homer", persons[0].Name);
            Assert.AreEqual(35, persons[0].Age);

            Assert.AreEqual("Marge", persons[1].Name);
            Assert.AreEqual(28, persons[1].Age);
        }

        [Test()]
        public void ResetTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));

            // Force to consume all the records
            foreach (var _ in reader.ReadAll()) { }

            Assert.IsTrue(reader.IsDone);
            reader.Reset();
            Assert.IsFalse(reader.IsDone);

            var enumerator = reader.ReadAll().GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Homer,35", enumerator.Current.ToString());

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Marge,28", enumerator.Current.ToString());

            Assert.IsFalse(enumerator.MoveNext());
            Assert.IsTrue(reader.IsDone);
        }

        [Test()]
        public void TryResetTest()
        {
            using var csv = StreamHelper.ToMemoryStream(
                "name,age\n" +
                "Homer,35\n" +
                "Marge,28\n");

            using var reader = new CsvReader(new StreamReader(csv));

            // Force to consume all the records
           foreach(var _ in reader.ReadAll()) { }

            Assert.IsTrue(reader.IsDone);            
            Assert.IsTrue(reader.TryReset());
            Assert.IsFalse(reader.IsDone);

            var enumerator = reader.ReadAll().GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Homer,35", enumerator.Current.ToString());

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Marge,28", enumerator.Current.ToString());

            Assert.IsFalse(enumerator.MoveNext());
            Assert.IsTrue(reader.IsDone);
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

        class Person
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }
    }
}