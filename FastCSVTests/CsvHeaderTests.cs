using NUnit.Framework;
using System;

namespace FastCSV.Tests
{
    [TestFixture()]
    public class CsvHeaderTests
    {
        [Test()]
        public void CsvHeaderTest()
        {
            var header = new CsvHeader(new string[] { "id", "name", "age" });

            Assert.AreEqual(3, header.Length);
            Assert.AreEqual(CsvFormat.Default, header.Format);

            Assert.AreEqual("id", header[0]);
            Assert.AreEqual("name", header[1]);
            Assert.AreEqual("age", header[2]);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = header[3];
            });
        }

        [Test()]
        public void CsvHeaderTest1()
        {
            var format = new CsvFormat("\t", "\"");
            var header = new CsvHeader(new string[] { "id", "name", "age" }, format);

            Assert.AreEqual(3, header.Length);
            Assert.AreEqual(new CsvFormat("\t", "\""), header.Format);

            Assert.AreEqual("id", header[0]);
            Assert.AreEqual("name", header[1]);
            Assert.AreEqual("age", header[2]);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = header[3];
            });
        }

        [Test()]
        public void CsvHeaderFromTest()
        {
            var header = CsvHeader.FromValues("id", "name", "age" );

            Assert.AreEqual(3, header.Length);
            Assert.AreEqual(CsvFormat.Default, header.Format);

            Assert.AreEqual("id", header[0]);
            Assert.AreEqual("name", header[1]);
            Assert.AreEqual("age", header[2]);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = header[3];
            });
        }

        public class Person1
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public class Person2
        {
            [CsvField("first_name")]
            public string Name { get; set; }
            [CsvField("age")]
            public int Age { get; set; }
        }

        [Test()]
        public void FromTypeTest()
        {
            var header = CsvHeader.FromType<Person1>();
            Assert.AreEqual(CsvHeader.FromValues("Name", "Age"), header);
        }

        [Test()]
        public void FromTypeWithAliasTest()
        {
            var header = CsvHeader.FromType<Person2>();
            Assert.AreEqual(CsvHeader.FromValues("first_name", "age"), header);
        }

        [Test()]
        public void IndexOfTest()
        {
            var header = new CsvHeader(new string[] { "id", "name", "age" });

            Assert.AreEqual(0, header.IndexOf("id"));
            Assert.AreEqual(1, header.IndexOf("name"));
            Assert.AreEqual(2, header.IndexOf("age"));
            Assert.AreEqual(-1, header.IndexOf("last name"));
        }

        [Test]
        public void WithDelimiterTest()
        {
            var header = CsvHeader.FromValues("name", "age");
            var copy = header.WithDelimiter(";");

            Assert.AreEqual(CsvFormat.Default.WithDelimiter(";"), copy.Format);
        }

        [Test]
        public void WithQuoteTest()
        {
            var header = CsvHeader.FromValues("name", "age");
            var copy = header.WithQuote("|");

            Assert.AreEqual(CsvFormat.Default.WithQuote("|"), copy.Format);
        }

        [Test]
        public void WithStyleTest()
        {
            var header = CsvHeader.FromValues("name", "age");
            var copy = header.WithStyle(QuoteStyle.Always);

            Assert.AreEqual(CsvFormat.Default.WithStyle(QuoteStyle.Always), copy.Format);
        }

        [Test]
        public void WithFormatTest()
        {
            var header = CsvHeader.FromValues("name", "age");
            var copy = header.WithFormat(new CsvFormat(";", "|"));

            Assert.AreEqual(new CsvFormat(";", "|"), copy.Format);
        }

        [Test()]
        public void ToStringTest()
        {
            var header = new CsvHeader(new string[] { "id", "name", "age" });
            Assert.AreEqual("id,name,age", header.ToString());
        }

        [Test()]
        public void ToStringTest1()
        {
            var header = new CsvHeader(new string[] { "id", "name", "age" });
            var format = CsvFormat.Default.WithDelimiter(";");
            Assert.AreEqual("id;name;age", header.ToString(format));
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            var header = new CsvHeader(new string[] { "id", "name", "age" });
            var enumerator = header.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("id", enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("name", enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("age", enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void CloneTest()
        {
            var header = new CsvHeader(new string[] { "id", "name", "age" });
            var clone = header.Clone();
            Assert.AreEqual(header, clone);
        }

        [Test()]
        public void EqualsTest()
        {
            var header = new CsvHeader(new string[] { "id", "name", "age" });
            Assert.AreEqual(CsvHeader.FromValues("id", "name", "age"), header);
        }
    }
}