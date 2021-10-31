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
    }
}
