﻿#nullable enable

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
            using var csv = StreamHelper.CreateStreamFromString(
                $"name,age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

            using var reader = new CsvReader(new StreamReader(csv));

            Assert.AreEqual(CsvFormat.Default, reader.Format);
            Assert.IsTrue(reader.HasHeader);
            Assert.IsFalse(reader.IsDone);
        }

        [Test()]
        public void CsvReaderTest1()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                    $"name,age{System.Environment.NewLine}" +
                    $"Homer,35{System.Environment.NewLine}" +
                    $"Marge,28{System.Environment.NewLine}");

            var format = new CsvFormat("\t", "\"");
            using var reader = new CsvReader(new StreamReader(csv), format);

            Assert.AreEqual(new CsvFormat("\t", "\""), reader.Format);
            Assert.IsTrue(reader.HasHeader);
            Assert.IsFalse(reader.IsDone);
        }

        [Test()]
        public void CsvReaderTest2()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                    $"name,age{System.Environment.NewLine}" +
                    $"Homer,35{System.Environment.NewLine}" +
                    $"Marge,28{System.Environment.NewLine}");

            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false);

            Assert.AreEqual(CsvFormat.Default, reader.Format);
            Assert.IsFalse(reader.HasHeader);
            Assert.IsFalse(reader.IsDone);
        }

        [Test()]
        public void FromStreamTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                    $"name,age{System.Environment.NewLine}" +
                    $"Homer,35{System.Environment.NewLine}" +
                    $"Marge,28{System.Environment.NewLine}");

            using var reader = new CsvReader(csv);

            Assert.AreEqual(CsvFormat.Default, reader.Format);
            Assert.IsTrue(reader.HasHeader);
            Assert.IsFalse(reader.IsDone);
        }

        [Test()]
        public void ReadTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"name,age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

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
            using var csv = StreamHelper.CreateStreamFromString("");

            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false);
            Assert.IsNull(reader.Read());
        }

        [Test()]
        public void ReadBlankTest1()
        {
            using var csv = StreamHelper.CreateStreamFromString(" ");

            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false);
            Assert.IsNull(reader.Read());
        }

        [Test()]
        public void ReadBlankTest2()
        {
            using var csv = StreamHelper.CreateStreamFromString(" ");

            var format = CsvFormat.Default.WithIgnoreWhitespace(false);
            using var reader = new CsvReader(new StreamReader(csv), hasHeader: false, format: format);
            Assert.AreEqual(" ", reader.Read()!.ToString());
        }

        [Test()]
        public void ReadRecordWithWhiteSpaceTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"Name,Age{System.Environment.NewLine}" +
                $"Homer , 35{System.Environment.NewLine}" +
                $" Marge,28{System.Environment.NewLine}");

            using var reader = new CsvReader(csv, CsvFormat.Default.WithIgnoreWhitespace(false));

            Assert.AreEqual("Homer , 35", reader.Read()!.ToString());
            Assert.AreEqual(" Marge,28", reader.Read()!.ToString());
        }

        [Test()]
        public void ReadWithUnclosedQuoteTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"Name,Age{System.Environment.NewLine}" +
                $"Mario \"The plumber, 20{System.Environment.NewLine}" +
                $"Luigi, 19{System.Environment.NewLine}");

            using var reader = new CsvReader(csv);

            Assert.Throws<CsvFormatException>(() =>
            {
                var _ = reader.Read();
            });
        }

        [Test()]
        public void ReadWithQuoteAlwaysTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"Name,Age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

            var reader = new CsvReader(csv, CsvFormat.Default.WithStyle(QuoteStyle.Always));

            Assert.AreEqual("\"Homer\",\"35\"", reader.Read()!.ToString());
            Assert.AreEqual("\"Marge\",\"28\"", reader.Read()!.ToString());
        }

        [Test()]
        public void ReadWithQuoteNeverTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"Name,Age{System.Environment.NewLine}" +
                $"\"Frida \"\"The Painter\"\"\", 35{System.Environment.NewLine}" +
                $"\"Pagannini \"\"The violinist\"\"\",28{System.Environment.NewLine}");

            var reader = new CsvReader(csv, CsvFormat.Default.WithStyle(QuoteStyle.Never));
            Assert.AreEqual("Frida The Painter,35", reader.Read()!.ToString());
            Assert.AreEqual("Pagannini The violinist,28", reader.Read()!.ToString());
        }

        [Test()]
        public async Task ReadAsyncTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"name,age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

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
            using var csv = StreamHelper.CreateStreamFromString(
                $"name,age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

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
            using var csv = StreamHelper.CreateStreamFromString(
                $"name,age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

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
        public void ReadAsTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"Name,Age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

            using var reader = new CsvReader(csv);

            Person p1 = reader.ReadAs<Person>().Value;
            Assert.AreEqual("Homer", p1.Name);
            Assert.AreEqual(35, p1.Age);

            Person p2 = reader.ReadAs<Person>().Value;
            Assert.AreEqual("Marge", p2.Name);
            Assert.AreEqual(28, p2.Age);

            Assert.IsFalse(reader.ReadAs<Person>().HasValue);
        }

        [Test]
        public void ReadAllAsTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"Name,Age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

            using var reader = new CsvReader(csv);

            var persons = reader.ReadAllAs<Person>().ToList();

            Assert.AreEqual("Homer", persons[0].Name);
            Assert.AreEqual(35, persons[0].Age);

            Assert.AreEqual("Marge", persons[1].Name);
            Assert.AreEqual(28, persons[1].Age);
        }

        [Test]
        public async Task ReadAsAsyncTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"Name,Age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

            using var reader = new CsvReader(csv);

            Person p1 = (await reader.ReadAsAsync<Person>()).Value;
            Assert.AreEqual("Homer", p1.Name);
            Assert.AreEqual(35, p1.Age);

            Person p2 = (await reader.ReadAsAsync<Person>()).Value;
            Assert.AreEqual("Marge", p2.Name);
            Assert.AreEqual(28, p2.Age);

            Assert.IsFalse((await reader.ReadAsAsync<Person>()).HasValue);
        }

        [Test]
        public async Task ReadAllAsAsyncTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"Name,Age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

            using var reader = new CsvReader(csv);
            var persons = reader.ReadAllAsAsync<Person>();

            await persons.MoveNextAsync();
            Assert.AreEqual("Homer", persons.Current.Name);
            Assert.AreEqual(35, persons.Current.Age);

            await persons.MoveNextAsync();
            Assert.AreEqual("Marge", persons.Current.Name);
            Assert.AreEqual(28, persons.Current.Age);
        }


        [Test()]
        public void ResetTest()
        {
            using var csv = StreamHelper.CreateStreamFromString(
                $"name,age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

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
            using var csv = StreamHelper.CreateStreamFromString(
                $"name,age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

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

        [Test]
        public void ReadAllChangeFormatTest()
        {
            using var csvStream = StreamHelper.CreateStreamFromString(
                $"name,age{System.Environment.NewLine}" +
                $"Homer,35{System.Environment.NewLine}" +
                $"Marge,28{System.Environment.NewLine}");

            using var reader = new CsvReader(csvStream);
            var readRecords = reader.ReadAll(new CsvFormat(delimiter: "|")).ToArray();

            Assert.AreEqual("Homer|35", readRecords[0].ToString());
            Assert.AreEqual("Marge|28", readRecords[1].ToString());
        }

        [Test()]
        public void CloseTest()
        {
            using var csv = new MemoryStream();
            var reader = new CsvReader(csv);
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
            var reader = new CsvReader(csv);
            reader.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                var _ = reader.Read();
            });
        }

        [Test]
        public void ReadStringCsvFormatTest()
        {
            string newLine = Environment.NewLine;
            using var stream = StreamHelper.CreateStreamFromString($"Name||Age{newLine}Maria||23{newLine}Juan || 20{newLine}");
            var format = new CsvFormat("||");
            using var reader = new CsvReader(stream, format);

            CollectionAssert.AreEqual(new string[] { "Name", "Age" }, reader.Header);
            CollectionAssert.AreEqual(new string[] { "Maria", "23" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "Juan", "20" }, reader.Read());
        }

        class Person
        {
            public string? Name { get; set; }
            public int Age { get; set; }
        }
    }
}