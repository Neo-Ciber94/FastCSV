using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void NewCsvColumnsTest()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var columns = record.AsRow();

            Assert.IsFalse(columns.IsEmpty);
            Assert.AreEqual(5, columns.Length);
            Assert.AreEqual(CsvFormat.Default, columns.Format);
        }

        [Test]
        public void SliceTest1()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var columns = record.AsRow();

            var slice = columns.Slice(0, 3);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual("2", slice[0]);
            Assert.AreEqual("Chair", slice[1]);
            Assert.AreEqual("450", slice[2]);
        }

        [Test]
        public void SliceTest2()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var columns = record.AsRow();

            var slice = columns.Slice(1, 3);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual("Chair", slice[0]);
            Assert.AreEqual("450", slice[1]);
            Assert.AreEqual("true", slice[2]);
        }

        [Test]
        public void SliceTest3()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var columns = record.AsRow();

            var slice = columns.Slice(^3);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual("450", slice[0]);
            Assert.AreEqual("true", slice[1]);
            Assert.AreEqual("Red", slice[2]);
        }


        [Test]
        public void SliceTest4()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var columns = record.AsRow();

            var slice = columns.Slice(2..5);

            Assert.AreEqual(3, slice.Length);
            Assert.AreEqual("450", slice[0]);
            Assert.AreEqual("true", slice[1]);
            Assert.AreEqual("Red", slice[2]);
        }

        [Test]
        public void IndexerTest1()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var columns = record.AsRow();

            Assert.AreEqual("2", columns[0]);
            Assert.AreEqual("Chair", columns[1]);
            Assert.AreEqual("450", columns[2]);
            Assert.AreEqual("true", columns[3]);
            Assert.AreEqual("Red", columns[4]);            
        }

        [Test]
        public void IndexerTest2()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var columns = record.AsRow();

            Assert.AreEqual("2", columns[^5]);
            Assert.AreEqual("Chair", columns[^4]);
            Assert.AreEqual("450", columns[^3]);
            Assert.AreEqual("true", columns[^2]);
            Assert.AreEqual("Red", columns[^1]);         
        }

        [Test]
        public void IndexerByKeyTest()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var columns = record.AsRow();

            Assert.AreEqual("2", columns["Id"]);
            Assert.AreEqual("Chair", columns["Name"]);
            Assert.AreEqual("450", columns["Price"]);
            Assert.AreEqual("true", columns["Available"]);
            Assert.AreEqual("Red", columns["Color"]);
        }

        [Test]
        public void GetEnumeratorTest()
        {
            var record = CsvRecord.From(new Product(2, "Chair", 450, true, "Red"));
            var columns = record.AsRow();

            var enumerator = columns.GetEnumerator();

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
