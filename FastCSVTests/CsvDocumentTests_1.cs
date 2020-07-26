using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FastCSV;
using NUnit.Framework;

namespace FastCsvTests
{
    public class Person : IEquatable<Person>
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Person);
        }

        public bool Equals(Person other)
        {
            return other != null &&
                   Name == other.Name &&
                   Age == other.Age;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age);
        }

        public static bool operator ==(Person left, Person right)
        {
            return EqualityComparer<Person>.Default.Equals(left, right);
        }

        public static bool operator !=(Person left, Person right)
        {
            return !(left == right);
        }
    }

    [TestFixture()]
    public class CsvDocumentTests1
    {
        [Test()]
        public void CsvDocumentTest()
        {
            var document = new CsvDocument<Person>();

            Assert.AreEqual(0, document.Count);
            Assert.IsTrue(document.IsEmpty);
            Assert.AreEqual(CsvHeader.FromValues("Name", "Age"), document.Header);
            Assert.AreEqual(CsvFormat.Default, document.Format);
        }

        [Test()]
        public void CsvDocumentTest1()
        {
            var format = new CsvFormat(';', '\"');
            var document = new CsvDocument<Person>(format);

            Assert.AreEqual(0, document.Count);
            Assert.IsTrue(document.IsEmpty);
            Assert.AreEqual(CsvHeader.FromValues(format, "Name", "Age"), document.Header);
            Assert.AreEqual(new CsvFormat(';', '\"'), document.Format);
        }

        [Test()]
        public void CsvDocumentTest2()
        {
            var persons = new List<Person>
            {
                new Person{ Name = "Akari", Age = 20 },
                new Person{ Name = "Kyoko", Age = 21 }
            };

            var document = new CsvDocument<Person>(persons);

            Assert.AreEqual(2, document.Count);
            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(CsvHeader.FromValues("Name", "Age"), document.Header);
            Assert.AreEqual(CsvFormat.Default, document.Format);
        }

        [Test()]
        public void CsvDocumentTest3()
        {
            var persons = new List<Person>
            {
                new Person{ Name = "Akari", Age = 20 },
                new Person{ Name = "Kyoko", Age = 21 }
            };

            var format = new CsvFormat(';', '\"');
            var document = new CsvDocument<Person>(persons, format);

            Assert.AreEqual(2, document.Count);
            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(CsvHeader.FromValues(format, "Name", "Age"), document.Header);
            Assert.AreEqual(new CsvFormat(';', '\"'), document.Format);
        }

        [Test]
        public void WriteTest()
        {
            var document = new CsvDocument<Person>();
            document.Write(new Person { Name = "Akari", Age = 20 });
            document.Write(new Person { Name = "Mirakurun", Age = 19 });

            Assert.AreEqual(2, document.Count);
            Assert.IsFalse(document.IsEmpty);

            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Akari", "20" }), document[0]);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Mirakurun", "19" }), document[1]);
        }

        [Test]
        public void WriteAtTest()
        {
            var document = new CsvDocument<Person>();
            document.Write(new Person { Name = "Akari", Age = 20 });
            document.Write(new Person { Name = "Mirakurun", Age = 19 });

            document.WriteAt(1, new Person { Name = "Yui", Age = 20 });
            Assert.AreEqual(3, document.Count);
            Assert.IsFalse(document.IsEmpty);

            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Akari", "20" }), document[0]);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Yui", "20" }), document[1]);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Mirakurun", "19" }), document[2]);
        }

        [Test]
        public void UpdateTest()
        {
            var document = new CsvDocument<Person>();
            document.Write(new Person { Name = "Akari", Age = 20 });
            document.Write(new Person { Name = "Mirakurun", Age = 19 });

            document.Update(1, new Person { Name = "Yui", Age = 20 });

            Assert.AreEqual(2, document.Count);
            Assert.IsFalse(document.IsEmpty);

            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Akari", "20" }), document[0]);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Yui", "20" }), document[1]);
        }

        [Test]
        public void RemoveAtTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            { 
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(4, document.Count);

            document.RemoveAt(0);

            Assert.AreEqual(3, document.Count);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                document.RemoveAt(3);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                document.RemoveAt(-1);
            });

            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Kyoko", "21" }), document[0]);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Yui", "22" }), document[1]);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Chinatsu", "19" }), document[2]);
        }

        [Test]
        public void RemoveAllTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(4, document.Count);
            Assert.AreEqual(2, document.RemoveAll(e => e.Age > 20));
            Assert.AreEqual(2, document.Count);


            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Akari", "20" }), document[0]);
            Assert.AreEqual(new CsvRecord(document.Header, new string[] { "Chinatsu", "19" }), document[1]);
        }

        [Test]
        public void ClearTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(4, document.Count);
            document.Clear();
            Assert.IsTrue(document.IsEmpty);
            Assert.AreEqual(0, document.Count);
        }

        [Test]
        public void ContainsTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.IsTrue(document.Contains(new Person { Name = "Akari", Age = 20 }));
            Assert.IsTrue(document.Contains(new Person { Name = "Kyoko", Age = 21 }));
            Assert.IsTrue(document.Contains(new Person { Name = "Yui", Age = 22 }));
            Assert.IsTrue(document.Contains(new Person { Name = "Chinatsu", Age = 19 }));
        }

        [Test]
        public void IndexOfTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(0, document.IndexOf(new Person { Name = "Akari", Age = 20 }));
            Assert.AreEqual(1, document.IndexOf(new Person { Name = "Kyoko", Age = 21 }));
            Assert.AreEqual(2, document.IndexOf(new Person { Name = "Yui", Age = 22 }));
            Assert.AreEqual(3, document.IndexOf(new Person { Name = "Chinatsu", Age = 19 }));

            Assert.AreEqual(-1, document.IndexOf(new Person { Name = "Ayano", Age = 19 }));
        }

        [Test]
        public void LastIndexOfTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(0, document.LastIndexOf(new Person { Name = "Akari", Age = 20 }));
            Assert.AreEqual(1, document.LastIndexOf(new Person { Name = "Kyoko", Age = 21 }));
            Assert.AreEqual(2, document.LastIndexOf(new Person { Name = "Yui", Age = 22 }));
            Assert.AreEqual(3, document.LastIndexOf(new Person { Name = "Chinatsu", Age = 19 }));

            Assert.AreEqual(-1, document.LastIndexOf(new Person { Name = "Ayano", Age = 19 }));
        }

        [Test]
        public void GetValueTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(new Person { Name = "Akari", Age = 20 }, document.GetValue(0));
            Assert.AreEqual(new Person { Name = "Kyoko", Age = 21 }, document.GetValue(1));
            Assert.AreEqual(new Person { Name = "Yui", Age = 22 }, document.GetValue(2));
            Assert.AreEqual(new Person { Name = "Chinatsu", Age = 19 }, document.GetValue(3));

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var _ = document.GetValue(-1);
            });
        }

        [Test]
        public void ValuesTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            var values = document.Values;
            Assert.AreEqual(4, values.Count);
            Assert.AreEqual(2, values.Count(p => p.Age > 20));
        }

        [Test]
        public void WithFormatTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            var copy = document.WithFormat(new CsvFormat(';', '"'));

            Assert.AreEqual(
                "Name;Age\r\n" +
                "Akari;20\r\n" +
                "Kyoko;21\r\n" +
                "Yui;22\r\n" +
                "Chinatsu;19\r\n", copy.ToString());
        }

        [Test]
        public void ToStringTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(
                "Name,Age\r\n" +
                "Akari,20\r\n" +
                "Kyoko,21\r\n" +
                "Yui,22\r\n" +
                "Chinatsu,19\r\n", document.ToString());
        }


        [Test]
        public void ToStringTest1()
        {
            var format = new CsvFormat(';', '"');
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(
                "Name;Age\r\n" +
                "Akari;20\r\n" +
                "Kyoko;21\r\n" +
                "Yui;22\r\n" +
                "Chinatsu;19\r\n", document.ToString(format));
        }

        [Test]
        public void GetEnumeratorTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
            });

            var enumerator = document.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(CsvRecord.From(new { Name = "Akari", Age = 20 }), enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(CsvRecord.From(new { Name = "Kyoko", Age = 21 }), enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(CsvRecord.From(new { Name = "Yui", Age = 22 }), enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
        }
    }
}
