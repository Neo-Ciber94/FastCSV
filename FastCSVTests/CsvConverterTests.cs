using NUnit.Framework;
using FastCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Collections;

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
    }
}