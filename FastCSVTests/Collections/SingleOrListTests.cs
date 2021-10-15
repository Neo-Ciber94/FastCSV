using NUnit.Framework;
using System;

namespace FastCSV.Collections.Tests
{
    [TestFixture]
    public class SingleOrListTests
    {
        [Test]
        public void AddTest()
        {
            var values = new SingleOrList<string>("fruits");
            Assert.AreEqual(1, values.Count);

            values.Add("vegetables");
            values.Add("grains");

            Assert.AreEqual("fruits", values[0]);
            Assert.AreEqual("vegetables", values[1]);
            Assert.AreEqual("grains", values[2]);
            Assert.AreEqual(3, values.Count);
        }

        [Test]
        public void AddWithResizeTest()
        {
            var values = new SingleOrList<string>("fruits");
            Assert.AreEqual(1, values.Count);

            values.Add("vegetables");
            values.Add("grains");
            values.Add("meats");
            values.Add("sweets");
            values.Add("cakes");
            values.Add("bread");
            values.Add("noodles");
            values.Add("soup");
            values.Add("mushroom");

            Assert.AreEqual("fruits", values[0]);
            Assert.AreEqual("vegetables", values[1]);
            Assert.AreEqual("grains", values[2]);
            Assert.AreEqual("meats", values[3]);
            Assert.AreEqual("sweets", values[4]);
            Assert.AreEqual("cakes", values[5]);
            Assert.AreEqual("bread", values[6]);
            Assert.AreEqual("noodles", values[7]);
            Assert.AreEqual("soup", values[8]);
            Assert.AreEqual("mushroom", values[9]);

            Assert.AreEqual(10, values.Count);
        }

        [Test]
        public void AddCollectionTest()
        {
            var values = new SingleOrList<string>(new string[] { "red", "blue", "green" });
            Assert.AreEqual(3, values.Count);

            values.Add("pink");
            Assert.AreEqual(4, values.Count);
        }

        [Test]
        public void InsertTest()
        {
            var values = SingleOrList<string>.Empty;
            Assert.AreEqual(0, values.Count);

            values.Add("1");
            values.Insert(0, "0");
            values.Insert(0, "-1");

            Assert.AreEqual(new string[] { "-1", "0", "1" }, values);
        }

        [Test]
        public void InsertIntoSingleTest()
        {
            var values = new SingleOrList<string>("colors");
            values.Insert(0, "red");

            Assert.AreEqual("red", values[0]);
            Assert.AreEqual("colors", values[1]);
        }

        [Test]
        public void RemoveTest()
        {
            var values = new SingleOrList<string>(new string[] { "1", "2", "3", "4" });
            Assert.True(values.Remove("2"));
            Assert.True(values.Remove("4"));

            Assert.AreEqual(2, values.Count);
        }

        [Test]
        public void RemoveAtTest()
        {
            var values = new SingleOrList<string>(new string[] { "1", "2", "3", "4" });
            values.RemoveAt(0);
            values.RemoveAt(2);

            CollectionAssert.AreEqual(new string[] { "2", "3" }, values);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                values.RemoveAt(-1);
            });

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                values.RemoveAt(3);
            });

            Assert.AreEqual(2, values.Count);
        }

        [Test]
        public void ClearTest()
        {
            var colors = new SingleOrList<int>(100);
            Assert.AreEqual(1, colors.Count);

            colors.Clear();
            Assert.AreEqual(0, colors.Count);
        }

        [Test]
        public void ClearFromEnumerableTest()
        {
            var numbers = new SingleOrList<int>(new int[] { 1, 2, 3});
            Assert.AreEqual(3, numbers.Count);

            numbers.Clear();
            Assert.AreEqual(0, numbers.Count);
        }

        [Test]
        public void ContainsTest()
        {
            SingleOrList<string> fruits = "apple";

            Assert.True(fruits.Contains("apple"));
            Assert.False(fruits.Contains("tomato"));

            fruits.Add("tomato");
            Assert.True(fruits.Contains("tomato"));
        }

        [Test]
        public void IndexOfTest()
        {
            SingleOrList<string> values = new string[] { "1", "2", "3", "4" };

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
            SingleOrList<string> values = "rock";

            Assert.AreEqual(0, values.IndexOf("rock"));
            Assert.AreEqual(-1, values.IndexOf("papper"));
        }

        [Test]
        public void CopyToTest()
        {
            var values = new SingleOrList<string>("blue");
            var array = new string[3];

            values.CopyTo(array, 1);

            Assert.AreEqual(new string[] { null, "blue", null }, array);
        }

        [Test]
        public void CopyToFromCollectionTest()
        {
            var values = new SingleOrList<string>(new string[] { "white", "gray", "black" });
            var array = new string[3];

            values.CopyTo(array, 0);

            Assert.AreEqual(new string[] { "white", "gray", "black" }, array);
        }

        [Test]
        public void IndexerTest()
        {
            var values = new SingleOrList<string>(new string[] { "red", "blue", "green" });

            Assert.AreEqual("red", values[0]);
            Assert.AreEqual("blue", values[1]);
            Assert.AreEqual("green", values[2]);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var _ = values[3];
            });

            values[0] = "0";
            values[1] = "1";
            values[2] = "2";

            Assert.AreEqual("0", values[0]);
            Assert.AreEqual("1", values[1]);
            Assert.AreEqual("2", values[2]);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                values[3] = "3";
            });
        }
    }
}
