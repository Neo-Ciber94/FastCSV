using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Utils;
using NUnit.Framework;

namespace FastCSV.Extensions.Tests
{
    [TestFixture]
    public class StringBuilderExtensionsTests
    {
        [Test]
        public void TrimStartTest()
        {
            Assert.AreEqual("Hello", new StringBuilder("  Hello").TrimStart().ToString());
            Assert.AreEqual("Hello", new StringBuilder("Hello").TrimStart().ToString());
            Assert.AreEqual("", new StringBuilder("    ").TrimStart().ToString());
        }

        [Test]
        public void TrimEndTest()
        {
            Assert.AreEqual("World", new StringBuilder("World  ").TrimEnd().ToString());
            Assert.AreEqual("World", new StringBuilder("World").TrimEnd().ToString());
            Assert.AreEqual("", new StringBuilder("     ").TrimEnd().ToString());
        }

        [Test]
        public void TrimTest()
        {
            Assert.AreEqual("Hello World", new StringBuilder("  Hello World ").Trim().ToString());
            Assert.AreEqual("Hello World", new StringBuilder("Hello World").Trim().ToString());
            Assert.AreEqual("", new StringBuilder("     ").Trim().ToString());
        }

        [Test]
        public void SliceTest()
        {
            Assert.AreEqual("Hello", new StringBuilder("Hello World").Slice(0, 5).ToString());
            Assert.AreEqual("World", new StringBuilder("Hello World").Slice(6, 5).ToString());

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new StringBuilder("Hello World").Slice(5, 10);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new StringBuilder("Hello World").Slice(0, 20);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new StringBuilder("Hello World").Slice(10, -2);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new StringBuilder("Hello World").Slice(-5, 4);
            });
        }

        [Test]
        public void SliceStartIndexTest()
        {
            Assert.AreEqual("World", new StringBuilder("Hello World").Slice(6).ToString());
            Assert.AreEqual("", new StringBuilder("Hello World").Slice(11).ToString());
        }

        [Test]
        public void SliceRangeTest()
        {
            Assert.AreEqual("Hello", new StringBuilder("Hello World").Slice(0..5).ToString());
            Assert.AreEqual("World", new StringBuilder("Hello World").Slice(6..11).ToString());
        }

        [Test]
        public void SliceIndexTest()
        {
            Assert.AreEqual("World", new StringBuilder("Hello World").Slice(^5).ToString());
            Assert.AreEqual("Hello World", new StringBuilder("Hello World").Slice(^11).ToString());
        }
    }
}
