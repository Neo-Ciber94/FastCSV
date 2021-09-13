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

        [Test]
        public void SerializeDeserializeDepth8Test()
        {
            var data = new A(new B(new C(new D(new E(new F(new G(new H(10))))))));
            var csv = CsvConverter.Serialize(data, DefaultOptions);

            Assert.AreEqual("Value\n10", csv);

            var result = CsvConverter.Deserialize<A>(csv, DefaultOptions);
            Assert.AreEqual(data, result);
        }

        [Test]
        public void SerializeTooDeepTest()
        {
            var data = new X(new Y(new A(new B(new C(new D(new E(new F(new G(new H(10))))))))));

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = CsvConverter.Serialize(data, DefaultOptions);
            });
        }

        record Product(string Name, Pricing Price);

        record Pricing(string Currency, decimal Price);

        record A(B B);
        record B(C C);
        record C(D D);
        record D(E E);
        record E(F F);
        record F(G G);
        record G(H H);
        record H(int Value);

        record X(Y Y);
        record Y(A A);
    }
}
