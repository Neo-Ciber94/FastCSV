using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastCSV.Tests
{
    [TestFixture()]
    public class CsvRecordTests
    {
        [Test()]
        public void CsvRecordTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });

            Assert.AreEqual(2, record.Length);
            Assert.AreEqual(CsvFormat.Default, record.Format);
            Assert.IsNull(record.Header);

            Assert.AreEqual("Violet", record[0]);
            Assert.AreEqual("16", record[1]);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var _ = record[2];
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = record["name"];
            });
        }

        [Test()]
        public void CsvRecordTest1()
        {
            var header = CsvHeader.FromValues("name", "age");
            var record = new CsvRecord(header, new string[] { "Violet", "16" });

            Assert.AreEqual(2, record.Length);
            Assert.AreEqual(CsvFormat.Default, record.Format);
            Assert.AreEqual(CsvHeader.FromValues("name", "age"), header);

            Assert.AreEqual("Violet", record[0]);
            Assert.AreEqual("16", record[1]);
            Assert.AreEqual("Violet", record["name"]);
            Assert.AreEqual("16", record["age"]);
        }

        [Test]
        public void FromTest()
        {
            var record = CsvRecord.From(new { Name = "Violet", Age = 16 });

            Assert.AreEqual(CsvHeader.FromValues("Name", "Age"), record.Header);
            Assert.AreEqual("Violet,16", record.ToString());
        }

        [Test()]
        public void WithDelimiterTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            var record2 = record.WithDelimiter('\t');

            Assert.AreEqual(CsvFormat.Default.WithDelimiter('\t'), record2.Format);
        }

        [Test()]
        public void WithQuoteTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            var record2 = record.WithQuote('\'');

            Assert.AreEqual(CsvFormat.Default.WithQuote('\''), record2.Format);
        }

        [Test()]
        public void WithStyleTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            var record2 = record.WithStyle(QuoteStyle.Always);

            Assert.AreEqual(CsvFormat.Default.WithStyle(QuoteStyle.Always), record2.Format);
        }

        [Test()]
        public void WithFormatTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            var record2 = record.WithFormat(new CsvFormat(';', '"'));

            Assert.AreEqual(new CsvFormat(';', '"'), record2.Format);
        }

        [Test]
        public void ToDictionaryTest()
        {
            var header = CsvHeader.FromValues("Name", "Age");
            var record = new CsvRecord(header, new string[] { "Maria", "20" });

            var dictionary = record.ToDictionary();
            Assert.AreEqual("Maria", dictionary["Name"]);
            Assert.AreEqual("20", dictionary["Age"]);
        }

        [Test()]
        public void ToStringTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            Assert.AreEqual("Violet,16", record.ToString());
        }

        [Test()]
        public void ToStringTest1()
        {
            var format = CsvFormat.Default.WithDelimiter('\t');
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            Assert.AreEqual("Violet\t16", record.ToString(format));
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            var enumerator = record.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Violet", enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("16", enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void CloneTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            var clone = record.Clone();
            Assert.AreEqual(clone, record);
        }

        [Test()]
        public void EqualsTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            Assert.AreEqual(new CsvRecord(null, new string[] { "Violet", "16" }), record);
        }
    }
}