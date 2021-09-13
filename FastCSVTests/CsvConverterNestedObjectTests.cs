using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvConverterNestedObjectTests
    {
        private static readonly CsvConverterOptions DefaultOptions = new CsvConverterOptions { NestedObjectHandling = NestedObjectHandling.Default };
        [Test]
        public void SerializeNestedObjectTest()
        {
            var product = new Product("Keyboard", new Pricing("Dollars", 2000));
            var csv = CsvConverter.Serialize(product, DefaultOptions);

            Assert.AreEqual("Name,Currency,Price\nKeyboard,Dollars,2000", csv);
        }

        [Test]
        public void DeserializeNestedObjectTest()
        {
            var csv = "Name,Currency,Price\nKeyboard,Dollars,2000";
            var product = CsvConverter.Deserialize<Product>(csv, DefaultOptions);

            Assert.AreEqual(new Product("Keyboard", new Pricing("Dollars", 2000)), product);
        }

        record Product(string Name, Pricing Price);

        record Pricing(string Currency, decimal Price);
    }
}
