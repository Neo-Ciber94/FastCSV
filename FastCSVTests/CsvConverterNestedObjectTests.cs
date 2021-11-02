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

            Assert.AreEqual($"Name,Currency,Price{System.Environment.NewLine}Keyboard,Dollars,2000", csv);
        }

        [Test]
        public void DeserializeNestedObjectTest()
        {
            var csv = $"Name,Currency,Price{System.Environment.NewLine}Keyboard,Dollars,2000";
            var product = CsvConverter.Deserialize<Product>(csv, DefaultOptions);

            Assert.AreEqual(new Product("Keyboard", new Pricing("Dollars", 2000)), product);
        }

        [Test]
        public void SerializeDeserializeDepth8Test()
        {
            var data = new A(new B(new C(new D(new E(new F(new G(new H(10))))))));
            var csv = CsvConverter.Serialize(data, DefaultOptions);

            Assert.AreEqual($"Value{System.Environment.NewLine}10", csv);

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

        [Test]
        public void ThrowErrorOnReferenceLoopTest()
        {
            var node = new ParentNode
            {
                Value = 50,
                Right = new Node
                {
                    Value = 40,
                    Left = new Node
                    {
                        Value = 30,
                    }
                }
            };

            var options = new CsvConverterOptions
            {
                NestedObjectHandling = new NestedObjectHandling
                {
                    MaxDepth = 10,
                    ReferenceLoopHandling = ReferenceLoopHandling.Error
                }
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = CsvConverter.Serialize(node, options);
            }, "Reference loop detected in property 'Left'");
        }

        [Test]
        public void IgnoreReferenceLoopTest()
        {
            var node = new ParentNode
            {
                Value = 50,
                Right = new Node
                {
                    Value = 40,
                    Left = new Node
                    {
                        Value = 30,
                    }
                }
            };

            var options = new CsvConverterOptions
            {
                NestedObjectHandling = new NestedObjectHandling
                {
                    MaxDepth = 10,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            };

            var csv = CsvConverter.Serialize(node, options);
            Assert.AreEqual($"Value,Value{Environment.NewLine}50,40", csv);
        }

        [Test]
        public void SerializeReferenceLoopTest()
        {
            var node = new ParentNode
            {
                Value = 50,
                Right = new Node
                {
                    Value = 40,
                    Left = new Node
                    {
                        Value = 30,
                    }
                }
            };

            var options = new CsvConverterOptions
            {
                NestedObjectHandling = new NestedObjectHandling
                {
                    MaxDepth = 10,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                }
            };

            var csv = CsvConverter.Serialize(node, options);
            Assert.AreEqual($"Value,Value,Value{Environment.NewLine}50,40,30", csv);
        }

        record Product(string Name, Pricing Price);

        record Pricing(string Currency, decimal Price);

        class ParentNode
        {
            public int Value { get; set; }
            public Node Right { get; set; }
        }

        class Node
        {
            public int Value { get; set; }
            public Node Left { get; set; }
        }

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
