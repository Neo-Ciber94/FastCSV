using NUnit.Framework;
using System;

namespace FastCSV.Collections.Tests
{
    [TestFixture]
    public class ReadOnlyReadOnlyValueOrListTests
    {
        [Test]
        public void ContainsTest()
        {
            ReadOnlyValueOrList<string> fruits = new string[] { "apple", "tomato" };

            Assert.True(fruits.Contains("apple"));
            Assert.True(fruits.Contains("tomato"));
        }

        [Test]
        public void IndexOfTest()
        {
            ReadOnlyValueOrList<string> values = new string[] { "1", "2", "3", "4" };

            Assert.AreEqual(0, values.IndexOf("1"));
            Assert.AreEqual(1, values.IndexOf("2"));
            Assert.AreEqual(2, values.IndexOf("3"));
            Assert.AreEqual(3, values.IndexOf("4"));

            Assert.AreEqual(-1, values.IndexOf("0"));
            Assert.AreEqual(-1, values.IndexOf("5"));
        }

        [Test]
        public void IndexOfSingleValueTest()
        {
            ReadOnlyValueOrList<string> values = "rock";

            Assert.AreEqual(0, values.IndexOf("rock"));
            Assert.AreEqual(-1, values.IndexOf("papper"));
        }

        [Test]
        public void CopyToTest()
        {
            var values = new ReadOnlyValueOrList<string>("blue");
            var array = new string[3];

            values.CopyTo(array, 1);

            Assert.AreEqual(new string[] { null, "blue", null }, array);
        }

        [Test]
        public void CopyToFromCollectionTest()
        {
            var values = new ReadOnlyValueOrList<string>(new string[] { "white", "gray", "black" });
            var array = new string[3];

            values.CopyTo(array, 0);

            Assert.AreEqual(new string[] { "white", "gray", "black" }, array);
        }

        [Test]
        public void IndexerTest()
        {
            var values = new ReadOnlyValueOrList<string>(new string[] { "red", "blue", "green" });

            Assert.AreEqual("red", values[0]);
            Assert.AreEqual("blue", values[1]);
            Assert.AreEqual("green", values[2]);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = values[3];
            });
        }

        [Test]
        public void ValueOrListConvertionTest()
        {
            var values = new ValueOrList<int>(new int[] { 1, 2, 3 });

            ReadOnlyValueOrList<int> readOnly = values;

            Assert.AreEqual(new int[] { 1, 2, 3 }, values);
            Assert.AreEqual(new int[] { 1, 2, 3 }, readOnly);
        }
    }
}
