﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

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

            Assert.Throws<ArgumentOutOfRangeException>(() =>
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
        public void IndexerTest1()
        {
            var record = CsvRecord.From(new
            {
                ID = 10,
                FirstName = "BoJack",
                LastName = "Horseman",
                Age = 50
            });

            Assert.AreEqual("10", record[0]);
            Assert.AreEqual("BoJack", record[1]);
            Assert.AreEqual("Horseman", record[2]);
            Assert.AreEqual("50", record[3]);
        }

        [Test()]
        public void IndexerTest2()
        {
            var record = CsvRecord.From(new
            {
                ID = 10,
                FirstName = "BoJack",
                LastName = "Horseman",
                Age = 50
            });

            Assert.AreEqual("10", record["ID"]);
            Assert.AreEqual("BoJack", record["FirstName"]);
            Assert.AreEqual("Horseman", record["LastName"]);
            Assert.AreEqual("50", record["Age"]);
        }

        [Test()]
        public void GetColumnsTest1()
        {
            var record = CsvRecord.From(new
            {
                ID = 10,
                FirstName = "BoJack",
                LastName = "Horseman",
                Age = 50
            });

            var values = record.GetColumns("ID", "FirstName");
            Assert.AreEqual("10", values["ID"]);
            Assert.AreEqual("BoJack", values["FirstName"]);
        }

        [Test()]
        public void GetColumnsTest2()
        {
            var record = CsvRecord.From(new
            {
                ID = 10,
                FirstName = "BoJack",
                LastName = "Horseman",
                Age = 50
            });

            var values = record.GetColumns("ID", "FirstName");
            Assert.AreEqual("10", values["ID"]);
            Assert.AreEqual("BoJack", values["FirstName"]);
        }

        [Test()]
        public void GetColumnsTest3()
        {
            var record = CsvRecord.From(new
            {
                ID = 10,
                FirstName = "BoJack",
                LastName = "Horseman",
                Age = 50
            });

            var values = record.GetColumns();
            Assert.AreEqual("10", values["ID"]);
            Assert.AreEqual("BoJack", values["FirstName"]);
            Assert.AreEqual("Horseman", values["LastName"]);
            Assert.AreEqual("50", values["Age"]);
        }

        [Test()]
        public void IndexerRangeTest()
        {
            var record = CsvRecord.From(new
            {
                ID = 10,
                FirstName = "BoJack",
                LastName = "Horseman",
                Age = 50
            });

            Assert.AreEqual(new string[] { "BoJack", "Horseman" }, record[1..3].ToArray());
        }

        [Test()]
        public void WithFormatTest()
        {
            var record = new CsvRecord(null, new string[] { "Violet", "16" });
            var record2 = record.WithFormat(new CsvFormat(";", "\""));

            Assert.AreEqual(new CsvFormat(";", "\""), record2.Format);
        }

        [Test()]
        public void MutateTest()
        {
            var record = CsvRecord.From(new Person { Name = "Jhon", Age = 20 });
            Assert.AreEqual("Jhon,20", record.ToString());

            var record2 = record.Mutate(self => self.Update("Age", 26));
            Assert.AreEqual("Jhon,26", record2.ToString());

            var record3 = record.Mutate(self =>
            {
                self[0] = "Carlos";
                self[1] = "30";
            });

            Assert.AreEqual("Carlos,30", record3.ToString());

            Assert.Throws<KeyNotFoundException>(() =>
            {
                var _ = record.Mutate(self =>
                {
                    self["LastName"] = "Lenon";
                });
            });
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
            var format = CsvFormat.Default.WithDelimiter("\t");
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

        class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}