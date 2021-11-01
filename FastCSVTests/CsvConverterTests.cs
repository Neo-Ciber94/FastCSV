using NUnit.Framework;
using FastCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Collections;
using FastCSV.Converters;

namespace FastCSV.Tests
{
    [TestFixture()]
    public class CsvConverterTests
    {
        record Product
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        record OtherProduct
        {
            public int Id { get; set; }
            public bool Available { get; set; }
        }

        [Test()]
        public void SerializeTest()
        {
            string csv = CsvConverter.Serialize(new Product { Name = "Table", Price = 200m }, typeof(Product));

            Assert.AreEqual($"Name,Price{System.Environment.NewLine}Table,200", csv);
        }

        [Test()]
        public void SerializeWithGenericsTest()
        {
            string csv = CsvConverter.Serialize(new Product { Name = "Table", Price = 200m });

            Assert.AreEqual($"Name,Price{System.Environment.NewLine}Table,200", csv);
        }

        [Test()]
        public void DeserializeTest()
        {
            var csv = $"Name,Price{System.Environment.NewLine}Table,200";
            Product product = CsvConverter.Deserialize(csv, typeof(Product)) as Product;

            Assert.AreEqual("Table", product.Name);
            Assert.AreEqual(200m, product.Price);
        }

        [Test()]
        public void DeserializeWithGenericsTest()
        {
            var csv = $"Name,Price{System.Environment.NewLine}Table,200";
            Product product = CsvConverter.Deserialize<Product>(csv);

            Assert.AreEqual("Table", product.Name);
            Assert.AreEqual(200m, product.Price);
        }

        [Test()]
        public void DeserializeHeaderMissmatchTest()
        {
            var csv = $"product_name,product_price{System.Environment.NewLine}Table,200";

            Assert.Throws<InvalidOperationException>(() =>
            {
                var product = CsvConverter.Deserialize(csv, typeof(Product));
            });
        }

        [Test()]
        public void DeserializeInvalidTypeTest()
        {
            var csv = $"Id,Available{System.Environment.NewLine}102,true";

            Assert.Throws<InvalidOperationException>(() =>
            {
                var product = CsvConverter.Deserialize(csv, typeof(Product));
            });
        }

        [Test()]
        public void GetValuesTest()
        {
            string[] values = CsvConverter.GetValues(new Product { Name = "Keyboard", Price = 2000m });

            Assert.AreEqual(new string[] { "Keyboard", "2000" }, values);
        }

        [Test()]
        public void GetHeaderTest()
        {
            string[] header = CsvConverter.GetHeader(typeof(Product));

            Assert.AreEqual(new string[] { "Name", "Price" }, header);
        }

        [Test]
        public void SerializeTypeWithoutConverterTest()
        {
            var options = new CsvConverterOptions
            {
                Converters = new List<ICsvValueConverter> { new PositiveNumberConverter() }
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = CsvConverter.Serialize(new Wrapper<PositiveNumber>(new PositiveNumber(23)));
            });

            Assert.AreEqual($"Value{System.Environment.NewLine}+(32)", CsvConverter.Serialize(new Wrapper<PositiveNumber>(new PositiveNumber(32)), options));
        }

        [Test]
        public void SerializeDeserializeReadOnlyFieldTest()
        {
            var options = new CsvConverterOptions
            {
                IncludeFields = true
            };

            Assert.AreEqual($"Value{System.Environment.NewLine}Hello", CsvConverter.Serialize(new ReadOnlyField<string>("Hello"), options));

            var result = CsvConverter.Deserialize<ReadOnlyField<string>>($"Value{System.Environment.NewLine}Hello", options);
            Assert.Null(result.Value);
        }

        [Test]
        public void SerializeDeserializeReadOnlyPropertyTest()
        {
            Assert.AreEqual($"Value{System.Environment.NewLine}Bye", CsvConverter.Serialize(new ReadOnlyProperty<string>("Bye")));

            var result = CsvConverter.Deserialize<ReadOnlyProperty<string>>($"Value{System.Environment.NewLine}Bye");
            Assert.Null(result.Value);
        }

        [Test]
        public void DeserializeToDifferentTypeTest()
        {
            var serialized = CsvConverter.Serialize(new Wrapper<string>("230"));
            Assert.AreEqual($"Value{System.Environment.NewLine}230", serialized);

            var deserialized = CsvConverter.Deserialize<Wrapper<int>>(serialized);
            Assert.AreEqual(230, deserialized.Value);
        }

        [Test]
        public void DeserializeUnorderedFieldsTest()
        {
            string csv = @"Working,Id,Name
true,10,Marie";

            var deserialized = CsvConverter.Deserialize<Person>(csv);
            Assert.AreEqual(new Person(10, "Marie", true), deserialized);
        }

        #region Helper Classes
        record Person(int Id, string Name, bool Working);

        record ReadOnlyField<T>
        {
            public readonly T Value;

            public ReadOnlyField(T value)
            {
                Value = value;
            }
        }

        record ReadOnlyProperty<T>
        {
            public T Value { get; }

            public ReadOnlyProperty(T value)
            {
                Value = value;
            }
        }

        record Wrapper<T>(T Value);

        record PositiveNumber(uint Number);

        class PositiveNumberConverter : ICsvCustomConverter<PositiveNumber>
        {
            public string ConvertFrom(PositiveNumber value)
            {
                return $"+({value.Number})";
            }

            public bool ConvertTo(ReadOnlySpan<char> s, out PositiveNumber value)
            {
                value = default!;

                if (s.Length > 3)
                {
                    if (s[0] == '+' && s[1] == '(' && s[^1] == ')')
                    {
                        var rest = s[2..^2];
                        if (uint.TryParse(rest, out uint result))
                        {
                            value = new PositiveNumber(result);
                            return true;
                        }
                    }
                }

                return false;
            }
        }
        #endregion
    }
}