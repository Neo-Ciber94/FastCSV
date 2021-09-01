using NUnit.Framework;
using FastCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Tests
{
    [TestFixture()]
    public class CsvConverterTests
    {
        class Product
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        class OtherProduct
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