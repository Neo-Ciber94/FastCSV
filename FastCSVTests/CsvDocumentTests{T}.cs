﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FastCSV.Converters;
using NUnit.Framework;

namespace FastCSV.Tests
{
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
            var format = new CsvFormat(";", "\"");
            var options = CsvConverterOptions.Default with { Format = format };
            var document = new CsvDocument<Person>(options);

            Assert.AreEqual(0, document.Count);
            Assert.IsTrue(document.IsEmpty);
            Assert.AreEqual(CsvHeader.FromValues(format, "Name", "Age"), document.Header);
            Assert.AreEqual(new CsvFormat(";", "\""), document.Format);
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

            var format = new CsvFormat(";", "\"");
            var options = CsvConverterOptions.Default with { Format = format };
            var document = new CsvDocument<Person>(persons, options);

            Assert.AreEqual(2, document.Count);
            Assert.IsFalse(document.IsEmpty);
            Assert.AreEqual(CsvHeader.FromValues(format, "Name", "Age"), document.Header);
            Assert.AreEqual(new CsvFormat(";", "\""), document.Format);
        }

        [Test()]
        public void FromCsvTest()
        {
            string csv = $"Name,Age{Environment.NewLine}" +
                    $"Red,23{Environment.NewLine}" +
                    $"Blue,24{Environment.NewLine}";

            var document = CsvDocument.FromCsv<Person>(csv);

            Assert.AreEqual(2, document.Count);
            Assert.AreEqual(new Person { Name = "Red", Age = 23 }, document.GetValue(0));
            Assert.AreEqual(new Person { Name = "Blue", Age = 24 }, document.GetValue(1));
        }

        [Test()]
        public void FromCsvTest1()
        {
            string csv = $"Name,Age,PhoneNumber{Environment.NewLine}" +
                $"Red,23,200-1200{Environment.NewLine}" +
                $"Blue,24,233-5565{Environment.NewLine}";

            var options = new CsvConverterOptions
            {
                Converters = new List<ICsvValueConverter>() { new PhoneNumberConverter() }
            };

            var document = CsvDocument.FromCsv<Employee>(csv, options: options);

            Assert.AreEqual(2, document.Count);
            Assert.AreEqual(new Employee("Red", 23, new PhoneNumber(2, 0, 0, 1, 2, 0, 0)), document.GetValue(0));
            Assert.AreEqual(new Employee("Blue", 24, new PhoneNumber(2, 3, 3, 5, 5, 6, 5)), document.GetValue(1));
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
                new Person {Name = "Chinatsu", Age = 19},
                new Person {Name = "Chitose", Age = 26},
            });

            Assert.AreEqual(5, document.Count);
            Assert.AreEqual(3, document.RemoveAll(e => e.Age > 20));
            Assert.AreEqual(2, document.Count);


            CollectionAssert.AreEqual(new CsvRecord(document.Header, new string[] { "Akari", "20" }), document[0]);
            CollectionAssert.AreEqual(new CsvRecord(document.Header, new string[] { "Chinatsu", "19" }), document[1]);
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

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = document.GetValue(-1);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = document.GetValue(4);
            });
        }

        [Test]
        public void GetValueRefTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(new Person { Name = "Kyoko", Age = 21 }, document.GetValueRef(1));
            Assert.AreEqual(new Person { Name = "Chinatsu", Age = 19 }, document.GetValueRef(3));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = document.GetValueRef(-1);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = document.GetValueRef(4);
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

            CsvDocument<Person> copy = document.WithFormat(new CsvFormat(";", "\""));
            string newLine = Environment.NewLine;

            Assert.AreEqual(
                $"Name;Age{newLine}" +
                $"Akari;20{newLine}" +
                $"Kyoko;21{newLine}" +
                $"Yui;22{newLine}" +
                $"Chinatsu;19{newLine}", copy.ToString());
        }

        [Test]
        public void WriteContentsToFileTest()
        {
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            using (var tempFile = new TempFile())
            {
                document.CopyToFile(tempFile.FullName);

                using var reader = new StreamReader(tempFile.FullName);
                Assert.AreEqual(
                   $"Name,Age{Environment.NewLine}" +
                   $"Akari,20{Environment.NewLine}" +
                   $"Kyoko,21{Environment.NewLine}" +
                   $"Yui,22{Environment.NewLine}" +
                   $"Chinatsu,19{Environment.NewLine}", reader.ReadToEnd());
            }
        }

        [Test]
        public void ReverseTests()
        {
            var document = new CsvDocument<Person>(new Person[]
{
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            document.Reverse();

            CollectionAssert.AreEqual(new Person[]
            {
                new Person {Name = "Chinatsu", Age = 19},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Akari", Age = 20},
            }, document.Values);
        }

        [Test]
        public void SortTest()
        {
            var document = new CsvDocument<int>(new[]
            {
                1, 5, 3, 2, 4
            });

            document.Sort();

            var array = document.Values.ToArray();
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, array);
        }

        [Test]
        public void SortByTest()
        {
            var document = new CsvDocument<Person>(new[]
            {
                new Person { Name = "Marie", Age = 18 },
                new Person { Name = "Carol", Age = 15 },
                new Person { Name = "Karen", Age = 24 }
            });

            document.SortBy(e => e.Age);

            CollectionAssert.AreEqual(new Person[]
            {
                new Person { Name = "Carol", Age = 15 },
                new Person { Name = "Marie", Age = 18 },
                new Person { Name = "Karen", Age = 24 },
            }, document.Values.ToArray());
        }

        [Test]
        public void ToCsvDocumentTest()
        {
            var document = new CsvDocument<Person>(new[]
{
                new Person { Name = "Marie", Age = 18 },
                new Person { Name = "Carol", Age = 15 },
                new Person { Name = "Karen", Age = 24 },
                new Person { Name = "Annie", Age = 30 },
            });

            CsvDocument<string> result = document.Values
                .Where(e => e.Age > 20)
                .Select(e => e.Name)
                .ToCsvDocument();

            string newLine = Environment.NewLine;
            Assert.AreEqual($"value{newLine}Karen{newLine}Annie{newLine}", result.ToString());
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
                $"Name,Age{Environment.NewLine}" +
                $"Akari,20{Environment.NewLine}" +
                $"Kyoko,21{Environment.NewLine}" +
                $"Yui,22{Environment.NewLine}" +
                $"Chinatsu,19{Environment.NewLine}", document.ToString());
        }

        [Test]
        public void ToStringTest1()
        {
            var format = new CsvFormat(";", "\"");
            var document = new CsvDocument<Person>(new Person[]
            {
                new Person {Name = "Akari", Age = 20},
                new Person {Name = "Kyoko", Age = 21},
                new Person {Name = "Yui", Age = 22},
                new Person {Name = "Chinatsu", Age = 19}
            });

            Assert.AreEqual(
                $"Name;Age{Environment.NewLine}" +
                $"Akari;20{Environment.NewLine}" +
                $"Kyoko;21{Environment.NewLine}" +
                $"Yui;22{Environment.NewLine}" +
                $"Chinatsu;19{Environment.NewLine}", document.ToString(format));
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

        /*
         * TEST UTILITY CLASSES
         */

        record Person : IEquatable<Person>
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        record Employee : IEquatable<Employee>
        {
            public Employee(string name, int age, PhoneNumber number)
            {
                Name = name;
                Age = age;
                PhoneNumber = number;
            }

            public string Name { get; set; }

            public int Age { get; set; }

            [CsvValueConverter(typeof(PhoneNumberConverter))]
            public PhoneNumber? PhoneNumber { get; set; }
        }

        struct PhoneNumber : IEquatable<PhoneNumber>
        {
            private readonly byte[] _number;

            public PhoneNumber(byte a, byte b, byte c, byte d, byte e, byte f, byte g)
            {
                _number = new byte[7] { a, b, c, d, e, f, g };
            }

            public PhoneNumber(byte[] number)
            {
                if (number.Length != 7)
                {
                    throw new ArgumentException("Expected length 7 phone number");
                }

                _number = number;
            }

            public override bool Equals(object obj)
            {
                return obj is PhoneNumber number && Equals(number);
            }

            public bool Equals(PhoneNumber other)
            {
                return _number.SequenceEqual(other._number);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_number);
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_number[0]);
                sb.Append(_number[1]);
                sb.Append(_number[2]);
                sb.Append("-");
                sb.Append(_number[3]);
                sb.Append(_number[4]);
                sb.Append(_number[5]);
                sb.Append(_number[6]);
                return sb.ToString();
            }

            public static bool operator ==(PhoneNumber left, PhoneNumber right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(PhoneNumber left, PhoneNumber right)
            {
                return !(left == right);
            }
        }

        class PhoneNumberConverter : ICsvCustomConverter<PhoneNumber>
        {
            public string ConvertFrom(PhoneNumber value)
            {
                return value.ToString();
            }

            public bool ConvertTo(ReadOnlySpan<char> s, out PhoneNumber value)
            {
                value = default;

                byte[] values = s.ToArray()
                    .Where(c => c != '-' && char.IsNumber(c))
                    .Select(c => (byte)char.GetNumericValue(c))
                    .ToArray();

                if (values.Length != 7)
                {
                    return false;
                }

                value = new PhoneNumber(values);
                return true;
            }
        }
    }
}
