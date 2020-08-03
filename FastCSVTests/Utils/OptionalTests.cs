using System;
using System.Collections.Generic;
using System.Linq;
using FastCSV.Utils;
using NUnit.Framework;

namespace FastCSV.Utils.Tests
{
    [TestFixture()]
    public class OptionalTests
    {
        [Test]
        public void HasValueTest()
        {
            var opt1 = new Optional<int>(10);
            Assert.IsTrue(opt1.HasValue);

            var opt2 = new Optional<int>();
            Assert.IsFalse(opt2.HasValue);
        }

        [Test]
        public void ValueTest()
        {
            var opt1 = new Optional<int>(10);
            Assert.AreEqual(10, opt1.Value);

            var opt2 = new Optional<int>();

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = opt2.Value;
            });
        }

        [Test]
        public void GetValueOrDefaultTest()
        {
            var opt1 = new Optional<int>(10);
            Assert.AreEqual(10, opt1.GetValueOrDefault(5));

            var opt2 = new Optional<int>();
            Assert.AreEqual(5, opt2.GetValueOrDefault(5));
        }

        [Test]
        public void ContainsTest()
        {
            var opt1 = new Optional<int>(10);
            Assert.IsTrue(opt1.Contains(10));
            Assert.IsFalse(opt1.Contains(7));

            var opt2 = new Optional<int>();
            Assert.IsFalse(opt2.Contains(5));
        }

        [Test]
        public void EqualityTest()
        {
            var opt1 = new Optional<int>(10);
            Assert.AreEqual(opt1, new Optional<int>(10));
            Assert.AreNotEqual(opt1, new Optional<int>(7));
        }

        [Test]
        public void FlattenTest()
        {
            var opt1 = new Optional<Optional<int>>(new Optional<int>(10));
            Assert.AreEqual(new Optional<int>(10), opt1.Flatten());

            var opt2 = new Optional<Optional<int>>();
            Assert.IsFalse(opt2.Flatten().HasValue);
        }

        [Test]
        public void MatchTest()
        {
            var opt1 = new Optional<int>(10);
            var r1 = opt1.Match(
                ifSome: (n) => n + 1,
                ifNone: () => { throw new Exception(); });

            Assert.AreEqual(new Optional<int>(11), r1);

            var opt2 = new Optional<int>();
            var r2 = opt2.Match(
                ifSome: (n) => n + 1,
                ifNone: () => { });

            Assert.IsFalse(r2.HasValue);
        }

        [Test]
        public void MapTest()
        {
            var opt1 = new Optional<int>(10);
            Assert.AreEqual(new Optional<int>(20), opt1.Map(n => n * 2));

            var opt2 = new Optional<int>();
            Assert.IsFalse(opt2.Map(n => n * 2).HasValue);
        }

        [Test]
        public void FilterTest()
        {
            var opt1 = new Optional<int>(10);
            Assert.AreEqual(new Optional<int>(10), opt1.Filter(n => n >= 10));

            var opt2 = new Optional<int>(5);
            Assert.IsFalse(opt2.Filter(n => n >= 10).HasValue);

            var opt3 = new Optional<int>();
            Assert.IsFalse(opt3.Filter(n => n >= 10).HasValue);
        }

        [Test]
        public void ImplicitCastTest()
        {
            var opt1 = Optional.Some(10);
            Assert.IsTrue(opt1 == 10);

            Optional<int> opt2 = 7;
            Assert.IsTrue(opt2 == 7);

            Optional<int> opt3 = default;
            Assert.Throws<InvalidOperationException>(() =>
            {
                int p = opt3;
            });
        }
    }
}
