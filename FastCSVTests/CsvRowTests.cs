using System;
using NUnit.Framework;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvRowTests
    {
        [Test]
        public void RecordOutOfRangeTest()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = record.AsRow(0, 10);
            });
        }

        [Test]
        public void HeaderOutOfRangeTest()
        {
            var header = CsvHeader.FromValues("Id", "Name", "Price", "Available", "Color");

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = header.AsRow(0, 10);
            });
        }

        [Test]
        public void NewCsvRowTest()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var row = record.AsRow();

            Assert.IsFalse(row.IsEmpty);
            Assert.AreEqual(5, row.Length);
            Assert.AreEqual(CsvFormat.Default, row.Format);
        }

        [Test]
        public void SliceTest1()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var row = record.AsRow();

            var slice = row.Slice(0, 3);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual("2", slice[0]);
            Assert.AreEqual("Chair", slice[1]);
            Assert.AreEqual("450", slice[2]);
        }

        [Test]
        public void SliceTest2()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var row = record.AsRow();

            var slice = row.Slice(1, 3);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual("Chair", slice[0]);
            Assert.AreEqual("450", slice[1]);
            Assert.AreEqual("true", slice[2]);
        }

        [Test]
        public void SliceTest3()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var row = record.AsRow();

            var slice = row.Slice(^3);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual("450", slice[0]);
            Assert.AreEqual("true", slice[1]);
            Assert.AreEqual("Red", slice[2]);
        }


        [Test]
        public void SliceTest4()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var row = record.AsRow();

            var slice = row.Slice(2..5);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual("450", slice[0]);
            Assert.AreEqual("true", slice[1]);
            Assert.AreEqual("Red", slice[2]);
        }

        [Test]
        public void IndexerTest1()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var row = record.AsRow();

            Assert.AreEqual("2", row[0]);
            Assert.AreEqual("Chair", row[1]);
            Assert.AreEqual("450", row[2]);
            Assert.AreEqual("true", row[3]);
            Assert.AreEqual("Red", row[4]);            
        }

        [Test]
        public void IndexerTest2()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var row = record.AsRow();

            Assert.AreEqual("2", row[^5]);
            Assert.AreEqual("Chair", row[^4]);
            Assert.AreEqual("450", row[^3]);
            Assert.AreEqual("true", row[^2]);
            Assert.AreEqual("Red", row[^1]);         
        }

        [Test]
        public void IndexerByKeyTest()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var row = record.AsRow();

            Assert.AreEqual("2", row["Id"]);
            Assert.AreEqual("Chair", row["Name"]);
            Assert.AreEqual("450", row["Price"]);
            Assert.AreEqual("true", row["Available"]);
            Assert.AreEqual("Red", row["Color"]);
        }

        [Test]
        public void GetEnumeratorTest()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var row = record.AsRow();

            var enumerator = row.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("2", enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Chair", enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("450", enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("true", enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual("Red", enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        record Product(int Id, string Name, decimal Price, bool Available, string Color);
    }
}
