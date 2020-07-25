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

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var _ = header[3];
            });
        }

        [Test()]
        public void CsvHeaderTest1()
        {
            var format = new CsvFormat('\t', '\'');
            var header = new CsvHeader(new string[] { "id", "name", "age" }, format);

            Assert.AreEqual(3, header.Length);
            Assert.AreEqual(new CsvFormat('\t', '\''), header.Format);

            Assert.AreEqual("id", header[0]);
            Assert.AreEqual("name", header[1]);
            Assert.AreEqual("age", header[2]);

            Assert.Throws<IndexOutOfRangeException>(() =>
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

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var _ = header[3];
            });
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
            var format = CsvFormat.Default.WithDelimiter(';');
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