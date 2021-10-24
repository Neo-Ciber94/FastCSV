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
        public void NextTest()
        {
            var cursor = new TextParser("Hello");

            Assert.AreEqual('H', cursor.Next().Value);
            Assert.AreEqual('e', cursor.Next().Value);
            Assert.AreEqual('l', cursor.Next().Value);
            Assert.AreEqual('l', cursor.Next().Value);
            Assert.AreEqual('o', cursor.Next().Value);
        }

        [Test]
        public void PeekTest()
        {
            var cursor = new TextParser("Hello");

            Assert.AreEqual('H', cursor.Peek().Value);
            Assert.AreEqual('H', cursor.Peek().Value);

            cursor.Next();
            Assert.AreEqual('e', cursor.Peek().Value);

            cursor.Next();
            Assert.AreEqual('l', cursor.Peek().Value);

            cursor.Next();
            Assert.AreEqual('l', cursor.Peek().Value);

            cursor.Next();
            Assert.AreEqual('o', cursor.Peek().Value);

            cursor.Next();
            Assert.False(cursor.Peek().HasValue);
        }

        [Test]
        public void RestTest()
        {
            var cursor = new TextParser("Hello");

            Assert.AreEqual("Hello", cursor.Rest.ToString());

            cursor.Next();
            Assert.AreEqual("ello", cursor.Rest.ToString());

            cursor.Next();
            Assert.AreEqual("llo", cursor.Rest.ToString());

            cursor.Next();
            Assert.AreEqual("lo", cursor.Rest.ToString());

            cursor.Next();
            Assert.AreEqual("o", cursor.Rest.ToString());

            cursor.Next();
            Assert.AreEqual("", cursor.Rest.ToString());
        }

        [Test]
        public void CanConsumeTest()
        {
            var cursor = new TextParser("Hello World");

            cursor.CanConsume("Hello");
            cursor.CanConsume("Hello World");

            cursor.Next();

            cursor.CanConsume("ello");
            cursor.CanConsume("ello World");

            cursor.Next();
            cursor.Next();
            cursor.Next();
            cursor.Next();
            cursor.Next();

            cursor.CanConsume("World");
        }

        [Test]
        public void ConsumeTest()
        {
            var cursor = new TextParser("Hello World");

            cursor.Consume("Hello ");
            Assert.AreEqual("World", cursor.Rest.ToString());
        }

        [Test]
        public void SliceStartTest()
        {
            var parser = new TextParser("Hello World");

            Assert.AreEqual("World", parser.Slice(6).Rest.ToString());

            Assert.AreEqual("Hello World", parser.Rest.ToString());
        }

        [Test]
        public void SliceStartCountTest()
        {
            var parser = new TextParser("Hello to my World");

            Assert.AreEqual("to my", parser.Slice(6, 5).Rest.ToString());

            Assert.AreEqual("Hello to my World", parser.Rest.ToString());
        }

        [Test]
        public void RangeIndexerTest()
        {
            var parser = new TextParser("Hello to my World");

            Assert.AreEqual("to my", parser[6..11].Rest.ToString());

            Assert.AreEqual("Hello to my World", parser.Rest.ToString());
        }
    }
}
