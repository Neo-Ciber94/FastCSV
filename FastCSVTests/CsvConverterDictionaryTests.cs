using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Collections;
using NUnit.Framework;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvConverterDictionaryTests
    {
        [Test]
        public void SerializeClassToDictionaryTest()
        {
            var product = new Product("Chair", 250.99m);
            var serialized = CsvConverter.SerializeToDictionary(product);

            Assert.AreEqual(2, serialized.Count);
            Assert.AreEqual("Chair", serialized["Name"]);
            Assert.AreEqual(250.99m, serialized["Price"]);
        }

        [Test]
        public void DeserializeDictionaryToClass()
        {
            var data = new Dictionary<string, SingleOrList<string>>()
            {
                { "Name", "Keyboard" },
                { "Price", "560.99" }
            };

            var deserialize = CsvConverter.DeserializeFromDictionary<Product>(data);
            Assert.AreEqual(new Product("Keyboard", 560.99m), deserialize);
        }

        [Test]
        public void SerializeStructToDictionaryTest()
        {
            var product = new ProductStruct("Chair", 250.99m);
            var serialized = CsvConverter.SerializeToDictionary(product);

            Assert.AreEqual(2, serialized.Count);
            Assert.AreEqual("Chair", serialized["Name"]);
            Assert.AreEqual(250.99m, serialized["Price"]);
        }

        [Test]
        public void DeserializeDictionaryToStruct()
        {
            var data = new Dictionary<string, SingleOrList<string>>()
            {
                { "Name", "Keyboard" },
                { "Price", "560.99" }
            };

            var deserialize = CsvConverter.DeserializeFromDictionary<ProductStruct>(data);
            Assert.AreEqual(new ProductStruct("Keyboard", 560.99m), deserialize);
        }

        [Test]
        public void SerializeCollectionToDictionaryTest()
        {
            var options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };
            var shoppingCar = new ShoppingCar(new string[] { "Mouse RGB", "Gaming Chair" }, 2);
            var serialized = CsvConverter.SerializeToDictionary(shoppingCar, options);

            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(serialized));
            Assert.AreEqual(3, serialized.Count);

            Assert.AreEqual("Mouse RGB", serialized["item1"]);
            Assert.AreEqual("Gaming Chair", serialized["item2"]);
            Assert.AreEqual(2, serialized["Count"]);
        }

        [Test]
        public void DeserializeCollectionToDictionaryTest()
        {
            var options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };
            var data = new Dictionary<string, SingleOrList<string>>()
            {
                { "Items", new string[]{ "Mouse RGB", "Gaming Chair" } },
                { "Count", "2" }
            };

            var deserialized = CsvConverter.DeserializeFromDictionary<ShoppingCar>(data, options);
            CollectionAssert.AreEqual(new string[] { "Mouse RGB", "Gaming Chair" }, deserialized.Items);
            Assert.AreEqual(2, deserialized.Count);
        }

        [Test]
        public void DeserializeCollectionToDictionaryTest2()
        {
            var options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };
            var data = new Dictionary<string, SingleOrList<string>>()
            {
                { "Codes", new string[]{ "231", "9901" } },
                { "Available", "true" }
            };

            var deserialized = CsvConverter.DeserializeFromDictionary<WarehouseItems>(data, options);
            CollectionAssert.AreEqual(new int[] { 231, 9901 }, deserialized.Codes);
            Assert.AreEqual(true, deserialized.Available);
        }

        [Test]
        public void SerializeNullableToDictionaryTests()
        {
            var values = CsvConverter.SerializeToDictionary(new Nullable<int>(10));
            Assert.AreEqual(values["value"], 10);
        }

        [Test]
        public void SerializeNullToDictionaryTest()
        {
            var values = CsvConverter.SerializeToDictionary(new Nullable<int>());
            Assert.AreEqual(values["value"], null);
        }

        [Test]
        public void DeserializeDictionaryToNullableTest()
        {
            var map = new Dictionary<string, SingleOrList<string>> { { "value", "20" } };
            var value = CsvConverter.DeserializeFromDictionary(map, typeof(Nullable<int>));

            Assert.AreEqual(20, value);
        }

        [Test]
        public void DeserializeNullDictionaryToNullableTest()
        {
            var map = new Dictionary<string, SingleOrList<string>> { { "value", "" } };
            var value = CsvConverter.DeserializeFromDictionary(map, typeof(Nullable<int>));

            Assert.AreEqual(null, value);
        }

        record Product(string Name, decimal Price);

        struct ProductStruct : IEquatable<ProductStruct>
        {
            public string Name { get; set; }
            public decimal Price { get; set; }

            public ProductStruct(string name, decimal price) => (Name, Price) = (name, price);

            public override bool Equals(object obj)
            {
                return obj is ProductStruct @struct && Equals(@struct);
            }

            public bool Equals(ProductStruct other)
            {
                return Name == other.Name &&
                       Price == other.Price;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Price);
            }

            public static bool operator ==(ProductStruct left, ProductStruct right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ProductStruct left, ProductStruct right)
            {
                return !(left == right);
            }
        }

        record ShoppingCar(string[] Items, int Count);

        record WarehouseItems(int[] Codes, bool Available);
    }
}
