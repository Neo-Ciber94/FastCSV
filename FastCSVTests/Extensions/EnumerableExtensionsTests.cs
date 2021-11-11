
using System;
using System.Linq;
using NUnit.Framework;

namespace FastCSV.Extensions.Tests
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void ContainsSequenceTest()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            Assert.IsTrue(array.ContainsSequence(new[] { 1 }));
            Assert.IsTrue(array.ContainsSequence(new[] { 1, 2 }));
            Assert.IsTrue(array.ContainsSequence(new[] { 1, 2, 3 }));
            Assert.IsTrue(array.ContainsSequence(new[] { 1, 2, 3, 4 }));
            Assert.IsTrue(array.ContainsSequence(new[] { 1, 2, 3, 4, 5 }));

            Assert.IsFalse(array.ContainsSequence(new[] { 1, 2, 3, 5 }));
            Assert.IsFalse(array.ContainsSequence(new[] { 1, 2, 3, 4, 5, 6 }));
            Assert.IsFalse(array.ContainsSequence(Array.Empty<int>()));
        }

        [Test]
        public void ContainsSequenceFromEmptyTest()
        {
            var array = Array.Empty<int>();

            Assert.IsFalse(array.ContainsSequence(Array.Empty<int>()));
            Assert.IsFalse(array.ContainsSequence(new[] { 1 }));
            Assert.IsFalse(array.ContainsSequence(new[] { 1, 2, 3 }));
        }
    }
}