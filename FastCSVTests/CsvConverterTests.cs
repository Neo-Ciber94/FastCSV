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

            Assert.AreEqual("Name,Price\nTable,200", csv);
        }

        [Test()]
        public void SerializeWithGenericsTest()
        {
            string csv = CsvConverter.Serialize(new Product { Name = "Table", Price = 200m });

            Assert.AreEqual("Name,Price\nTable,200", csv);
        }

        [Test()]
        public void DeserializeTest()
        {
            var csv = "Name,Price\nTable,200";
            Product product = CsvConverter.Deserialize(csv, typeof(Product)) as Product;

            Assert.AreEqual("Table", product.Name);
            Assert.AreEqual(200m, product.Price);
        }

        [Test()]
        public void DeserializeWithGenericsTest()
        {
            var csv = "Name,Price\nTable,200";
            Product product = CsvConverter.Deserialize<Product>(csv);

            Assert.AreEqual("Table", product.Name);
            Assert.AreEqual(200m, product.Price);
        }

        [Test()]
        public void DeserializeHeaderMissmatchTest()
        {
            var csv = "product_name,product_price\nTable,200";

            Assert.Throws<InvalidOperationException>(() =>
            {
                var product = CsvConverter.Deserialize(csv, typeof(Product));
            });
        }

        [Test()]
        public void DeserializeInvalidTypeTest()
        {
            var csv = "Id,Available\n102,true";

            Assert.Throws<InvalidOperationException>(() =>
            {
                var product = CsvConverter.Deserialize(csv, typeof(Product));
            });
        }

        [Test()]
        public void SerializeToDictionaryTest()
        {
            var product = new Product { Name = "Keyboard", Price = 1000m };
            var dictionary = CsvConverter.SerializeToDictionary(product, typeof(Product));

            Assert.AreEqual(dictionary["Name"], "Keyboard");
            Assert.AreEqual(dictionary["Price"], 1000m);
        }

        [Test()]
        public void SerializeToDictionaryGenericTest()
        {
            var product = new Product { Name = "Keyboard", Price = 1000m };
            var dictionary = CsvConverter.SerializeToDictionary<Product>(product);

            Assert.AreEqual(dictionary["Name"], "Keyboard");
            Assert.AreEqual(dictionary["Price"], 1000m);
        }

        [Test()]
        public void DeserializeFromDictionaryTest()
        {
            var dictionary = new Dictionary<string, SingleOrList<string>>
            {
                { "Name", "Keyboard" },
                { "Price", "1000" }
            };

            var product = CsvConverter.DeserializeFromDictionary(dictionary, typeof(Product));
            Assert.AreEqual(new Product { Name = "Keyboard", Price = 1000 }, product);
        }

        [Test()]
        public void DeserializeFromDictionaryGenericTest()
        {
            var dictionary = new Dictionary<string, SingleOrList<string>>
            {
                { "Name", "Keyboard" },
                { "Price", "1000" }
            };

            var product = CsvConverter.DeserializeFromDictionary<Product>(dictionary);
            Assert.AreEqual(new Product { Name = "Keyboard", Price = 1000 }, product);
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

            Assert.AreEqual("Value\n+(32)", CsvConverter.Serialize(new Wrapper<PositiveNumber>(new PositiveNumber(32)), options));
        }

        [Test]
        public void SerializeDeserializeReadOnlyFieldTest()
        {
            var options = new CsvConverterOptions
            {
                IncludeFields = true
            };

            Assert.AreEqual("Value\nHello", CsvConverter.Serialize(new ReadOnlyField<string>("Hello"), options));

            var result = CsvConverter.Deserialize<ReadOnlyField<string>>("Value\nHello", options);
            Assert.Null(result.Value);
        }

        [Test]
        public void SerializeDeserializeReadOnlyPropertyTest()
        {
            Assert.AreEqual("Value\nBye", CsvConverter.Serialize(new ReadOnlyProperty<string>("Bye")));

            var result = CsvConverter.Deserialize<ReadOnlyProperty<string>>("Value\nBye");
            Assert.Null(result.Value);
        }

        [Test]
        public void DeserializeToDifferentTypeTest()
        {
            var serialized = CsvConverter.Serialize(new Wrapper<string>("230"));
            Assert.AreEqual("Value\n230", serialized);

            var deserialized = CsvConverter.Deserialize<Wrapper<int>>(serialized);
            Assert.AreEqual(230, deserialized.Value);
        }

        #region Helper Classes

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

        class PositiveNumberConverter : IValueConverter<PositiveNumber>
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