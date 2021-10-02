using NUnit.Framework;
using System;

namespace FastCSV.Collections.Tests
{
    [TestFixture]
    public class ReadOnlyArrayTests
    {
        [Test]
        public void IndexerTest()
        {
            var array = new ReadOnlyArray<int>(new int[] { 1, 2, 3, 4 });
            Assert.AreEqual(4, array.Count);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var _ = array[-1];
            });

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var _ = array[4];
            });
        }

        [Test]
        public void IndexerRangeTest()
        {
            var array = new ReadOnlyArray<int>(new int[] { 1, 2, 3, 4 });

            Assert.AreEqual(new int[] { 1, 2 }, array[0..2].ToArray());
            Assert.AreEqual(new int[] { 3, 4 }, array[2..4].ToArray());
        }

        [Test]
        public void ContainsTest()
        {
            var array = new ReadOnlyArray<int>(new int[] { 1, 2, 3, 4 });

            Assert.True(array.Contains(1));
            Assert.True(array.Contains(2));
            Assert.True(array.Contains(3));
            Assert.True(array.Contains(4));

            Assert.False(array.Contains(0));
            Assert.False(array.Contains(5));
        }

        [Test]
        public void IndexOfTest()
        {
            var array = new ReadOnlyArray<int>(new int[] { 1, 2, 3, 4 });

            Assert.AreEqual(0, array.IndexOf(1));
            Assert.AreEqual(1, array.IndexOf(2));
            Assert.AreEqual(2, array.IndexOf(3));
            Assert.AreEqual(3, array.IndexOf(4));

            Assert.AreEqual(-1, array.IndexOf(0));
            Assert.AreEqual(-1, array.IndexOf(5));
        }

        [Test]
        public void ToArrayTest()
        {
            var readarray = new ReadOnlyArray<string>(new string[] { "red", "blue", "green" });
            var array = readarray.ToArray();

            Assert.AreEqual(array[0], readarray[0]);
            Assert.AreEqual(array[1], readarray[1]);
            Assert.AreEqual(array[2], readarray[2]);
        }

        [Test]
        public void IsEmptyTest()
        {
            ReadOnlyArray<int> readarray = default;

            Assert.True(readarray.IsEmpty);
        }

        [Test]
        public void EnumerableTest()
        {
            ReadOnlyArray<int> array = new int[] { 1, 2, 3, 4 };

            var enumerator = array.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual(4, enumerator.Current);

            Assert.False(enumerator.MoveNext());
        }

        [Test]
        public void EqualityTest()
        {
            var array = new ReadOnlyArray<int>(new int[] { 1, 2, 3, 4 });

            Assert.AreEqual(4, array.Count);
            Assert.AreEqual(new int[] { 1, 2, 3, 4 }, array);
        }

        [Test]
        public void ToStringTest()
        {
            var array = new ReadOnlyArray<int>(new int[] { 1, 2, 3, 4 });
            Assert.AreEqual("[1, 2, 3, 4]", array.ToString());
        }
    }
}