using NUnit.Framework;
using System;
using System.Collections.Generic;
using FastCSV.Extensions;

namespace FastCSV.Utils.Tests
{
    [TestFixture()]
    public class StringExtensionsTests
    {
        [Test()]
        public void EnclosedWithTest()
        {
            Assert.IsTrue("'Hello'".EnclosedWith("'"));
            Assert.IsTrue("sHellos".EnclosedWith("s"));

            Assert.IsFalse("'Hello".EnclosedWith("'"));
            Assert.IsFalse("Hellos".EnclosedWith("s"));
        }

        [Test()]
        public void EnclosedWithTest1()
        {
            Assert.IsTrue("'Hello'".EnclosedWith('\''));
            Assert.IsTrue("sHellos".EnclosedWith('s'));

            Assert.IsFalse("Hello'".EnclosedWith('\''));
            Assert.IsFalse("sHello".EnclosedWith('s'));
        }

        [Test()]
        public void IntoStringTest()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            Assert.AreEqual("[1,2,3,4,5]", list.IntoString());
            Assert.AreEqual("[1 - 2 - 3 - 4 - 5]", list.IntoString(separator: " - "));
            Assert.AreEqual("1,2,3,4,5", list.IntoString(encloseWithBrackets: false));
        }

        [Test()]
        public void IntoStringTest1()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };

            Assert.AreEqual("[1,2,3,4,5]", span.IntoString());
            Assert.AreEqual("[1 - 2 - 3 - 4 - 5]", span.IntoString(separator: " - "));
            Assert.AreEqual("1,2,3,4,5", span.IntoString(encloseWithBrackets: false));
        }

        [Test()]
        public void IntoStringTest2()
        {
            ReadOnlySpan<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };

            Assert.AreEqual("[1,2,3,4,5]", span.IntoString());
            Assert.AreEqual("[1 - 2 - 3 - 4 - 5]", span.IntoString(separator: " - "));
            Assert.AreEqual("1,2,3,4,5", span.IntoString(encloseWithBrackets: false));
        }
    }
}