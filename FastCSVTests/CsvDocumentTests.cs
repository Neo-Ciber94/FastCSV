﻿using System;
using System.IO;
using System.Linq;
using FastCSV.Tests;
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

        [Test]
        public void FromCsvTest()
        {
            var csv = $"Name,Age{Environment.NewLine}" +
                    $"Kara,20{Environment.NewLine}" +
                    $"Markus,25{Environment.NewLine}";

            CsvDocument document = CsvDocument.FromCsv(csv);

            Assert.AreEqual(2, document.Count);
            Assert.AreEqual(CsvHeader.FromValues("Name", "Age"), document.Header);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Kara", "20" }), document[0]);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Markus", "25" }), document[1]);
        }

        [Test]
        public void FromCsvTest1()
        {
            var csv = $"Name,Age{Environment.NewLine}" +
                    $"Kara,20{Environment.NewLine}" +
                    $"Markus,25,RK-200{Environment.NewLine}";

            var document = CsvDocument.FromCsv(csv, flexible: true);

            Assert.AreEqual(2, document.Count);
            Assert.AreEqual(CsvHeader.FromValues("Name", "Age"), document.Header);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Kara", "20" }), document[0]);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Markus", "25", "RK-200" }), document[1]);
        }

        [Test()]
        public void WriteTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            CollectionAssert.AreEqual(new string[] { "Light", "18" }, document[0]);
            CollectionAssert.AreEqual(new string[] { "Misa", "20" }, document[1]);

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void WriteAllTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.WriteAll(new string[] { "Light", "18" });
            document.WriteAll(new string[] { "Misa", "20" });

            CollectionAssert.AreEqual(new string[] { "Light", "18" }, document[0]);
            CollectionAssert.AreEqual(new string[] { "Misa", "20" }, document[1]);

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void WriteWithTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.WriteValue(new { Name = "Light", Age = "18" });
            document.WriteValue(new { Name = "Misa", Age = "20" });

            CollectionAssert.AreEqual(new string[] { "Light", "18" }, document[0]);
            CollectionAssert.AreEqual(new string[] { "Misa", "20" }, document[1]);

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void WriteAtTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            document.WriteAt(0, new string[] { "Ryuk", "999" });

            CollectionAssert.AreEqual(new string[] { "Ryuk", "999" }, document[0]);
            CollectionAssert.AreEqual(new string[] { "Light", "18" }, document[1]);
            CollectionAssert.AreEqual(new string[] { "Misa", "20" }, document[2]);

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(3, document.Count);
        }

        [Test()]
        public void WriteAllAtTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            document.WriteAllAt(0, new string[] { "Ryuk", "999" });

            CollectionAssert.AreEqual(new string[] { "Ryuk", "999" }, document[0]);
            CollectionAssert.AreEqual(new string[] { "Light", "18" }, document[1]);
            CollectionAssert.AreEqual(new string[] { "Misa", "20" }, document[2]);

            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(3, document.Count);
        }

        [Test()]
        public void WriteWithAtTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            document.WriteValueAt(0, new { Name = "Ryuk", Age = 999 });

            CollectionAssert.AreEqual(new string[] { "Ryuk", "999" }, document[0]);
            CollectionAssert.AreEqual(new string[] { "Light", "18" }, document[1]);
            CollectionAssert.AreEqual(new string[] { "Misa", "20" }, document[2]);

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

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = document[3];
            });
        }

        [Test()]
        public void UpdateTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.WriteValue(new { Name = "Light", Age = "18" });
            document.WriteValue(new { Name = "Misa", Age = "20" });

            document.Update(1, new string[] { "Misa", "17" });

            CollectionAssert.AreEqual(new string[] { "Light", "18" }, document[0]);
            CollectionAssert.AreEqual(new string[] { "Misa", "17" }, document[1]);

            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void UpdateWithTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.WriteValue(new { Name = "Light", Age = "18" });
            document.WriteValue(new { Name = "Misa", Age = "20" });

            document.UpdateValue(1, new { Name = "Misa", Age = 17 });

            CollectionAssert.AreEqual(new string[] { "Light", "18" }, document[0]);
            CollectionAssert.AreEqual(new string[] { "Misa", "17" }, document[1]);

            Assert.AreEqual(2, document.Count);
        }

        [Test()]
        public void MutateAtTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            document.MutateAt(0, record => record.Update("name", "Light Yagami"));

            CollectionAssert.AreEqual(new string[] { "Light Yagami", "18" }, document[0]);
            CollectionAssert.AreEqual(new string[] { "Misa", "20" }, document[1]);

            Assert.AreEqual("Light Yagami,18", document[0].ToString());
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
        public void GetColumnTest1()
        {
            string data = $"name,age\n" +
                $"Robert,40\n" +
                $"Tom,20\n";

            var csv = CsvDocument.FromCsv(data);

            var names = csv.GetColumn("name");
            Assert.AreEqual(new[] { "Robert", "Tom" }, names.ToArray());

            var ages = csv.GetColumn("age");
            Assert.AreEqual(new[] { "40", "20" }, ages.ToArray());
        }

        [Test()]
        public void GetColumnTest2()
        {
            string data = $"name,age\n" +
                $"Robert,40\n" +
                $"Tom,20\n";

            var csv = CsvDocument.FromCsv(data);

            var names = csv.GetColumn(0);
            Assert.AreEqual(new[] { "Robert", "Tom" }, names.ToArray());

            var ages = csv.GetColumn(1);
            Assert.AreEqual(new[] { "40", "20" }, ages.ToArray());
        }

        [Test()]
        public void GetColumnsTest1()
        {
            string data = $"name,age\n" +
                $"Robert,40\n" +
                $"Tom,20\n";

            var csv = CsvDocument.FromCsv(data);

            var columns = csv.GetColumns();
            Assert.AreEqual(new[] { "Robert", "Tom" }, columns.ElementAt(0).ToArray());
            Assert.AreEqual(new[] { "40", "20" }, columns.ElementAt(1).ToArray());
        }

        [Test()]
        public void GetColumnsTest2()
        {
            string data = $"name,age\n" +
                $"Robert,40\n" +
                $"Tom,20\n";

            var csv = CsvDocument.FromCsv(data);

            var columns = csv.GetColumns("age", "name");
            Assert.AreEqual(new[] { "Robert", "Tom" }, columns.ElementAt(1).ToArray());
            Assert.AreEqual(new[] { "40", "20" }, columns.ElementAt(0).ToArray());
        }

        [Test]
        public void WithFormatTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            var copy = document.WithFormat(new CsvFormat(";", "\""));
            Assert.AreEqual($"name;age{Environment.NewLine}Light;18{Environment.NewLine}Misa;20{Environment.NewLine}", copy.ToString());
        }

        [Test]
        public void WriteContentsToFileTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            using(var tempFile = new TempFile())
            {
                document.CopyToFile(tempFile.FullName);

                using var reader = new StreamReader(tempFile.FullName);
                Assert.AreEqual($"name,age{Environment.NewLine}Light,18{Environment.NewLine}Misa,20{Environment.NewLine}", reader.ReadToEnd());
            }
        }

        [Test()]
        public void ToStringTest()
        {
            var document = new CsvDocument(new string[] { "name", "age" });
            document.Write("Light", 18);
            document.Write("Misa", 20);

            Assert.AreEqual($"name,age{Environment.NewLine}Light,18{Environment.NewLine}Misa,20{Environment.NewLine}", document.ToString());
        }

        [Test()]
        public void ToStringTest1()
        {
            var format = new CsvFormat(";", "\"");
            var document = new CsvDocument(new string[] { "name", "age" }, format);
            document.Write("Light", 18);
            document.Write("Misa", 20);

            Assert.AreEqual($"name;age{Environment.NewLine}Light;18{Environment.NewLine}Misa;20{Environment.NewLine}", document.ToString(format));
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
    }
}