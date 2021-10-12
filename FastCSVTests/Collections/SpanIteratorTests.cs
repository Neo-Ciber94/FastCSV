using FastCSV.Utils;
using NUnit.Framework;
using System;

namespace FastCSV.Collections.Tests
{
    [TestFixture]
    public class SpanIteratorTests
    {
        [Test]
        public void GetEnumeratorTest()
        {
            ReadOnlySpan<int> span = stackalloc int[5] { 1, 2, 3, 4, 5 };

            var iterator = new SpanIterator<int>(span);
            var enumerator = iterator.GetEnumerator();

            Assert.True(enumerator.HasNext());
            enumerator.MoveNext();
            Assert.AreEqual(1, enumerator.Current);

            Assert.True(enumerator.HasNext());
            enumerator.MoveNext();
            Assert.AreEqual(2, enumerator.Current);

            Assert.True(enumerator.HasNext());
            enumerator.MoveNext();
            Assert.AreEqual(3, enumerator.Current);

            Assert.True(enumerator.HasNext());
            enumerator.MoveNext();
            Assert.AreEqual(4, enumerator.Current);

            Assert.True(enumerator.HasNext());
            enumerator.MoveNext();
            Assert.AreEqual(5, enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.False(enumerator.HasNext());
        }

        [Test]
        public void PeekTest()
        {
            ReadOnlySpan<int> span = stackalloc int[5] { 1, 2, 3, 4, 5 };

            var iterator = new SpanIterator<int>(span);

            Assert.AreEqual(1, iterator.Peek.Value);
            iterator.MoveNext();

            Assert.AreEqual(2, iterator.Peek.Value);
            iterator.MoveNext();

            Assert.AreEqual(3, iterator.Peek.Value);
            iterator.MoveNext();

            Assert.AreEqual(4, iterator.Peek.Value);
            iterator.MoveNext();

            Assert.AreEqual(5, iterator.Peek.Value);

            Assert.True(iterator.MoveNext());
            Assert.False(iterator.Peek.HasValue);
        }
    }
}
