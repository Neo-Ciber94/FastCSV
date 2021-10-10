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
    public class CsvConverterAttributesTests
    {
        record Product
        {
            [CsvField("name")]
            public string Name { get; set; }

            [CsvField("price")]
            public decimal Price { get; set; }
            
            [CsvIgnore]
            public int Amount { get; set; }
        }

        [Test]
        public void SerializeTest()
        {
            var product = new Product { Name = "PC", Price = 2000m, Amount = 3 };
            var csv = CsvConverter.Serialize(product, typeof(Product));

            Assert.AreEqual("name,price\nPC,2000", csv);
        }

        [Test]
        public void SerializeWithGenericsTest()
        {
            var product = new Product { Name = "PC", Price = 2000m, Amount = 3 };
            var csv = CsvConverter.Serialize<Product>(product);

            Assert.AreEqual("name,price\nPC,2000", csv);
        }

        [Test]
        public void DeserializeTest()
        {
            var csv = "name,price\nPC,2000";
            var product = CsvConverter.Deserialize(csv, typeof(Product));

            Assert.AreEqual(new Product { Name = "PC", Price = 2000m, Amount = default }, product);
        }

        [Test]
        public void DeserializeWithGenericsTest()
        {
            var csv = "name,price\nPC,2000";
            var product = CsvConverter.Deserialize<Product>(csv);

            Assert.AreEqual(new Product { Name = "PC", Price = 2000m, Amount = default }, product);
        }

        [Test]
        public void DeserializeWithIgnoredFieldTest()
        {
            var csv = "name,price,amount\nPC,2000,34";
            var product = CsvConverter.Deserialize(csv, typeof(Product));

            Assert.AreEqual(new Product { Name = "PC", Price = 2000m, Amount = default }, product);
        }
    }
}
