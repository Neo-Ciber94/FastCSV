using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Tests
{
    [TestFixture]
    public class CsvConverterCollectionHandlingTests
    {
        private static readonly CsvConverterOptions Options = new CsvConverterOptions
        {
            CollectionHandling = CollectionHandling.Default
        };

        [Test]
        public void SerializeObjectWithArrayTest()
        {
            var obj = new ShoppingCart(new string[] { "Apple", "Chips", "Chicken" }, 3);
            var csv = CsvConverter.Serialize(obj, Options);

            Assert.AreEqual("item1,item2,item3,Count\nApple,Chips,Chicken,3", csv);
        }

        [Test]
        public void DeserializeObjectWithArrayTest()
        {
            var csv = "item1,item2,item3,Count\nApple,Chips,Chicken,3";
            var obj = CsvConverter.Deserialize<ShoppingCart>(csv, Options);

            Assert.AreEqual(new ShoppingCart(new string[] { "Apple", "Chips", "Chicken" }, 3), obj);
        }

        record ShoppingCart(string[] Items, int Count);
    }
}
