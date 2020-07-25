using System;
using NUnit.Framework;

namespace FastCSV.Tests
{
    [TestFixture()]
    public class CsvDocumentTests
    {
        [Test()]
        public void CsvDocumentTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });

            Assert.AreEqual(CsvHeader.FromValues("name", "age"), document.Header);
            Assert.AreEqual(CsvFormat.Default, document.Format);
            Assert.IsFalse(document.IsFlexible);
            Assert.IsTrue(document.IsEmpty);
            Assert.AreEqual(0, document.Count);
        }

        [Test()]
        public void CsvDocumentTest1()
        {
            var document = new CsvDocument(new string[] { "name", "age" });

            Assert.AreEqual(CsvHeader.FromValues("name", "age"), document.Header);
            Assert.IsFalse(document.IsFlexible);
            Assert.IsTrue(document.IsEmpty);
            Assert.AreEqual(0, document.Count);
        }

        [Test()]
        public void CsvDocumentTest2()
        {
            var document = new CsvDocument(new string[] { "name", "age" }, flexible: true);

            Assert.AreEqual(CsvHeader.FromValues("name", "age"), document.Header);
            Assert.IsTrue(document.IsFlexible);
            Assert.IsTrue(document.IsEmpty);
            Assert.AreEqual(0, document.Count);
        }

        [Test()]
        public void WriteTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void WriteAllTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.WriteAll(new string[] { "Light", "18" });
            document.WriteAll(new string[] { "Misa", "20" });

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void WriteWithTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.WriteWith(new { Name = "Light", Age = "18" });
            document.WriteWith(new { Name = "Misa", Age = "20" });

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void WriteAtTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            document.WriteAt(0, new string[] { "Ryuu", "999" });

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(3, document.Count);
        }

        [Test()]
        public void WriteWithAtTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            document.WriteAtWith(0, new { Name = "Ryuu", Age = 999 });

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(3, document.Count);
        }

        [Test()]
        public void IndexerTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            var record1 = new CsvRecord(document.Header, new string[] { "Light", "18" });
            Assert.AreEqual(record1, document[0]);

            var record2 = new CsvRecord(document.Header, new string[] { "Misa", "20" });
            Assert.AreEqual(record2, document[1]);
        }

        [Test()]
        public void UpdateTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.WriteWith(new { Name = "Light", Age = "18" });
            document.WriteWith(new { Name = "Misa", Age = "20" });

            document.Update(1, new string[] { "Misa", "17" });
            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void UpdateWithTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.WriteWith(new { Name = "Light", Age = "18" });
            document.WriteWith(new { Name = "Misa", Age = "20" });

            document.UpdateWith(1, new { Name = "Misa", Age = 17 });
            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void RemoveAtTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            document.RemoveAt(0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                document.RemoveAt(-1);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                document.RemoveAt(1);
            });

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(1, document.Count);
        }

        [Test()]
        public void ClearTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(2, document.Count);

            document.Clear();

            Assert.IsTrue(document.IsEmpty);
            Assert.AreEqual(0, document.Count);
        }

        [Test()]
        public void ToStringTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            Assert.AreEqual("name,age\r\nLight,18\r\nMisa,20\r\n", document.ToString());
        }

        [Test()]
        public void ToStringTest1()
        {
            var format = new CsvFormat(';', '\"');
            var document = new CsvDocument(new string[] { "name", "age" }, format);
            document.Write("Light", 18);
            document.Write("Misa", 20);

            Assert.AreEqual("name;age\r\nLight;18\r\nMisa;20\r\n", document.ToString(format));
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            var enumerator = document.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Light,18", enumerator.Current.ToString());

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Misa,20", enumerator.Current.ToString());

            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test()]
        public void CsvDocumentTest3()
        {

        }
    }
}