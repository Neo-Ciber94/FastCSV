﻿using FastCSV;
using NUnit.Framework;

namespace FastCSVTests
{
    [TestFixture]
    public class CsvConverterOptionsTests
    {
        [Test]
        public void SerializeIncludeFieldTests()
        {
            var product = new ProductWithFields { name = "Battery", price = 50m };
            string csv = CsvConverter.Serialize(product, typeof(ProductWithFields), new CsvConverterOptions
            {
                IncludeFields = true
            });

            Assert.AreEqual("name,price,Available\nBattery,50,true", csv);
        }

        [Test]
        public void DeserializeIncludeFieldsTest()
        {
            string csv = "name,price,Available\nBattery,50,true";
            var product = (ProductWithFields)CsvConverter.Deserialize(csv, typeof(ProductWithFields), new CsvConverterOptions
            {
                IncludeFields = true
            });

            Assert.AreEqual(new ProductWithFields { name = "Battery", price = 50m }, product);
        }

        [Test]
        public void SerializeWithoutHeaderTest()
        {
            var product = new Product("Bat", 200m);
            string csv = CsvConverter.Serialize(product, typeof(Product), new CsvConverterOptions
            {
                IncludeHeader = false
            });

            Assert.AreEqual("Bat,200", csv);
        }

        [Test]
        public void DeserializeithoutHeaderTest()
        {
            string csv = "Bat,200";
            Product product = (Product)CsvConverter.Deserialize(csv, typeof(Product), new CsvConverterOptions
            {
                IncludeHeader = false
            });

            Assert.AreEqual(new Product("Bat", 200), product);
        }

        [Test]
        public void SerializeWithSnakeCaseNamingConventionTest()
        {
            var person = new Person("Kanna", "Kobayashi");
            person.currentCountry = "Japan";

            var csv = CsvConverter.Serialize(person, new CsvConverterOptions { 
                IncludeFields = true, 
                NamingConvention = CsvNamingConvention.SnakeCase 
            });

            // Autogenerated fields come last
            Assert.AreEqual("current_country,first_name,last_name\nJapan,Kanna,Kobayashi", csv);
        }

        record ProductWithFields
        {
            public string name;
            public decimal price;

            public bool Available { get; set; } = true;
        }

        record Product(string Name, decimal Price);

        record Person(string FirstName, string LastName)
        {
            public string currentCountry;
        }
    }
}