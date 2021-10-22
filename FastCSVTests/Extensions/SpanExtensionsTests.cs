using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Extensions.Tests
{
    [TestFixture]
    public class SpanExtensionsTests
    {
        [Test]
        public void IndexOfTest()
        {
            Span<int> span = new int[] { 1, 2, 3, 4, 5 };

            Assert.AreEqual(0, span.IndexOf(1));
            Assert.AreEqual(1, span.IndexOf(2));
            Assert.AreEqual(2, span.IndexOf(3));
            Assert.AreEqual(3, span.IndexOf(4));
            Assert.AreEqual(4, span.IndexOf(5));

            Assert.AreEqual(-1, span.IndexOf(6));
        }

        [Test]
        public void IndexOfSpanTest()
        {
            Span<int> span = new int[] { 1, 2, 3, 4, 5 };

            Assert.AreEqual(0, span.IndexOf(stackalloc int[] { 1, 2 }));
            Assert.AreEqual(1, span.IndexOf(stackalloc int[] { 2, 3 }));
            Assert.AreEqual(2, span.IndexOf(stackalloc int[] { 3, 4 }));
            Assert.AreEqual(3, span.IndexOf(stackalloc int[] { 4, 5 }));

            Assert.AreEqual(0, span.IndexOf(stackalloc int[] { 1, 2, 3 }));
            Assert.AreEqual(1, span.IndexOf(stackalloc int[] { 2, 3, 4 }));
            Assert.AreEqual(2, span.IndexOf(stackalloc int[] { 3, 4, 5 }));

            Assert.AreEqual(0, span.IndexOf(stackalloc int[] { 1, 2, 3, 4 }));
            Assert.AreEqual(1, span.IndexOf(stackalloc int[] { 2, 3, 4, 5 }));

            Assert.AreEqual(-1, span.IndexOf(stackalloc int[] { 5, 6 }));
            Assert.AreEqual(-1, span.IndexOf(stackalloc int[] { 1, 4 }));
            Assert.AreEqual(-1, span.IndexOf(stackalloc int[] { 3, 5 }));
        }

        [Test]
        public void LastIndexOfTest()
        {
            Span<int> span = new int[] { 1, 2, 3, 4, 5 };

            Assert.AreEqual(0, span.LastIndexOf(1));
            Assert.AreEqual(1, span.LastIndexOf(2));
            Assert.AreEqual(2, span.LastIndexOf(3));
            Assert.AreEqual(3, span.LastIndexOf(4));
            Assert.AreEqual(4, span.LastIndexOf(5));

            Assert.AreEqual(-1, span.LastIndexOf(6));
        }

        [Test]
        public void LastIndexOfSpanTest()
        {
            Span<int> span = new int[] { 1, 2, 3, 4, 5 };

            Assert.AreEqual(0, span.LastIndexOf(stackalloc int[] { 1, 2 }));
            Assert.AreEqual(1, span.LastIndexOf(stackalloc int[] { 2, 3 }));
            Assert.AreEqual(2, span.LastIndexOf(stackalloc int[] { 3, 4 }));
            Assert.AreEqual(3, span.LastIndexOf(stackalloc int[] { 4, 5 }));

            Assert.AreEqual(0, span.LastIndexOf(stackalloc int[] { 1, 2, 3 }));
            Assert.AreEqual(1, span.LastIndexOf(stackalloc int[] { 2, 3, 4 }));
            Assert.AreEqual(2, span.LastIndexOf(stackalloc int[] { 3, 4, 5 }));

            Assert.AreEqual(0, span.LastIndexOf(stackalloc int[] { 1, 2, 3, 4 }));
            Assert.AreEqual(1, span.LastIndexOf(stackalloc int[] { 2, 3, 4, 5 }));

            Assert.AreEqual(-1, span.LastIndexOf(stackalloc int[] { 5, 6 }));
            Assert.AreEqual(-1, span.LastIndexOf(stackalloc int[] { 1, 4 }));
            Assert.AreEqual(-1, span.LastIndexOf(stackalloc int[] { 3, 5 }));
        }

        [Test]
        public void ContainsTest()
        {
            Span<int> span = new int[] { 1, 2, 3, 4, 5 };
        }
    }
}
