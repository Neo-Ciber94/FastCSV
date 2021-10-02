using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Utils;
using NUnit.Framework;

namespace FastCSV.Utils.Tests
{
    [TestFixture]
    public class EitherTests
    {
        [Test]
        public void FromLeftTest()
        {
            Either<int, string> either = Either.FromLeft(10);

            Assert.IsTrue(either.IsLeft);
            Assert.IsFalse(either.IsRight);

            Assert.AreEqual(either.Left, 10);

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = either.Right;
            });

            Assert.AreEqual(either.ToString(), "Left(10)");
        }

        [Test]
        public void FromRightTest()
        {
            Either<int, string> either = Either.FromRight("hello");

            Assert.IsFalse(either.IsLeft);
            Assert.IsTrue(either.IsRight);

            Assert.AreEqual(either.Right, "hello");

            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = either.Left;
            });


            Assert.AreEqual(either.ToString(), "Right(hello)");
        }
    }
}
