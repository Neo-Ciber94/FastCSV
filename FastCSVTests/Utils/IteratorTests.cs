using NUnit.Framework;
using FastCSV.Utils;
using System.Collections.Generic;
using System;

namespace FastCSV.Tests.Utils
{
    [TestFixture()]
    public class IteratorTests
    {
        [Test]
        public void AsPeekableTest()
        {
            var elements = new List<int> { 1, 2, 3, 4, 5 };

            IIterator<int> enumerator = elements.GetEnumerator().AsIterator();

            Assert.AreEqual(1, enumerator.Peek.Value);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.HasNext());
            Assert.AreEqual(1, enumerator.Current);
            Assert.AreEqual(2, enumerator.Peek.Value);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.HasNext());
            Assert.AreEqual(2, enumerator.Current);
            Assert.AreEqual(3, enumerator.Peek.Value);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.HasNext());
            Assert.AreEqual(3, enumerator.Current);
            Assert.AreEqual(4, enumerator.Peek.Value);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.HasNext());
            Assert.AreEqual(4, enumerator.Current);
            Assert.AreEqual(5, enumerator.Peek.Value);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.HasNext());
            Assert.AreEqual(5, enumerator.Current);
            Assert.IsFalse(enumerator.Peek.HasValue);
        }
    }
}