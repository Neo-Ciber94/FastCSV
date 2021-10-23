using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FastCSV.Utils
{
    [TestFixture]
    public class TextParserTests
    {
        [Test]
        public void EnumeratorTest()
        {
            var parser = new TextParser("Green");

            Assert.True(parser.MoveNext());
            Assert.AreEqual('G', parser.Current);

            Assert.True(parser.MoveNext());
            Assert.AreEqual('r', parser.Current);

            Assert.True(parser.MoveNext());
            Assert.AreEqual('e', parser.Current);

            Assert.True(parser.MoveNext());
            Assert.AreEqual('e', parser.Current);

            Assert.True(parser.MoveNext());
            Assert.AreEqual('n', parser.Current);

            Assert.False(parser.MoveNext());
        }

        [Test]
        public void NextTest()
        {
            var parser = new TextParser("Green");

            Assert.True(parser.HasNext());
            Assert.AreEqual('G', parser.Next().Value);
            Assert.AreEqual('r', parser.Next().Value);
            Assert.AreEqual('e', parser.Next().Value);
            Assert.AreEqual('e', parser.Next().Value);
            Assert.AreEqual('n', parser.Next().Value);
            Assert.False(parser.MoveNext());
        }

        [Test]
        public void PeekTest()
        {
            var parser = new TextParser("Anima");

            Assert.AreEqual('A', parser.Peek().Value);
            parser.MoveNext();

            Assert.AreEqual('n', parser.Peek().Value);
            parser.MoveNext();

            Assert.AreEqual('i', parser.Peek().Value);
            parser.MoveNext();

            Assert.AreEqual('m', parser.Peek().Value);
            parser.MoveNext();

            Assert.AreEqual('a', parser.Peek().Value);
            parser.MoveNext();

            Assert.False(parser.Peek().HasValue);
        }

        [Test]
        public void RestTest()
        {
            var parser = new TextParser("Hello World");

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = parser.Rest.ToString();
            });

            parser.MoveNext();
            Assert.AreEqual("Hello World", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("ello World", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("llo World", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("lo World", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("o World", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual(" World", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("World", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("orld", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("rld", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("ld", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("d", parser.Rest.ToString());

            parser.MoveNext();
            Assert.AreEqual("", parser.Rest.ToString());

            Assert.False(parser.MoveNext());
            Assert.AreEqual(0, parser.Rest.Length);
        }

        [Test]
        public void CanConsumeTest()
        {
            var parser = new TextParser("Hello World");
            parser.MoveNext();

            Assert.True(parser.CanConsume("Hello"));
            Assert.True(parser.CanConsume("Hello World"));

            parser.MoveNext();
            Assert.False(parser.CanConsume("Hello"));
            Assert.False(parser.CanConsume("Hello World"));

            Assert.True(parser.CanConsume("ello"));
            Assert.True(parser.CanConsume("ello World"));

            Assert.False(parser.CanConsume(""));

            parser.MoveNext();
            parser.MoveNext();
            parser.MoveNext();
            parser.MoveNext();
            parser.MoveNext();
            parser.MoveNext();
            parser.MoveNext();
            parser.MoveNext();
            parser.MoveNext();
            parser.MoveNext();

            Assert.False(parser.CanConsume("d"));
            Assert.False(parser.CanConsume(""));
        }

        [Test]
        public void ConsumeTest()
        {
            var parser = new TextParser("Hello World");
            parser.MoveNext();

            Assert.AreEqual(1, parser.Consume("H"));
            Assert.AreEqual("ello World", parser.Rest.ToString());

            Assert.AreEqual(5, parser.Consume("ello "));
            Assert.AreEqual("World", parser.Rest.ToString());

            Assert.AreEqual(5, parser.Consume("World"));
            Assert.AreEqual("", parser.Rest.ToString());
        }

        [Test]
        public void SliceStartTest()
        {
            var parser = new TextParser("Hello World");
            parser.MoveNext();

            Assert.AreEqual("World", parser.Slice(6).Rest.ToString());

            Assert.AreEqual("Hello World", parser.Rest.ToString());
        }

        [Test]
        public void SliceStartCountTest()
        {
            var parser = new TextParser("Hello to my World");
            parser.MoveNext();

            Assert.AreEqual("to my", parser.Slice(6, 5).Rest.ToString());

            Assert.AreEqual("Hello to my World", parser.Rest.ToString());
        }

        [Test]
        public void RangeIndexerTest()
        {
            var parser = new TextParser("Hello to my World");
            parser.MoveNext();

            Assert.AreEqual("to my", parser[6..11].Rest.ToString());

            Assert.AreEqual("Hello to my World", parser.Rest.ToString());
        }
    }
}
