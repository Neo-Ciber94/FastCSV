using NUnit.Framework;
using System;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvNamingConventionTests
    {
        [Test]
        public void SnakeCaseSerializeDeserializeTest()
        {
            var options = new CsvConverterOptions
            {
                NamingConvention = CsvNamingConvention.SnakeCase
            };

            string serialized = CsvConverter.Serialize(new Product(239, "Hot Sauce", 399.99m), options);

            Assert.AreEqual($"id,name,price{Environment.NewLine}239,Hot Sauce,399.99", serialized);

            Product deserialized = CsvConverter.Deserialize<Product>(serialized, options);
            Assert.AreEqual(new Product(239, "Hot Sauce", 399.99m), deserialized);
        }

        [Test]
        public void CustomNamingSerializeDeserializeTest()
        {
            var options = new CsvConverterOptions
            {
                NamingConvention = new UpperCaseNamingConvention()
            };

            string serialized = CsvConverter.Serialize(new Product(239, "Hot Sauce", 399.99m), options);

            Assert.AreEqual($"ID,NAME,PRICE{Environment.NewLine}239,Hot Sauce,399.99", serialized);

            Product deserialized = CsvConverter.Deserialize<Product>(serialized, options);
            Assert.AreEqual(new Product(239, "Hot Sauce", 399.99m), deserialized);
        }

        record Product(int Id, string Name, decimal Price);

        class UpperCaseNamingConvention : CsvNamingConvention
        {
            public override string Convert(string name) => name.ToUpper();
        }
    }
}
