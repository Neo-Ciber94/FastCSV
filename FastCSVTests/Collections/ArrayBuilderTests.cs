using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Collections.Tests
{
    [TestFixture]
    public class ArrayBuilderTests
    {
        [Test]
        public void ToArrayTests()
        {
            using var builder = new ArrayBuilder<int>(10);

            builder.Add(1);
            builder.Add(2);
            builder.Add(3);
            builder.Add(4);
            builder.Add(5);

            Assert.AreEqual(5, builder.Count);
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, builder.ToArray());

            builder.Add(6);
            builder.Add(7);
            builder.Add(8);
            builder.Add(9);
            builder.Add(10);

            Assert.AreEqual(10, builder.Count);
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, builder.ToArray());
        }

        [Test]
        public void AddWithResizeTest()
        {
            const int Max = 1000_000;
            using var builder = new ArrayBuilder<int>(10);

            for (int i = 0; i < Max; i++)
            {
                builder.Add(i);
            }

            int[] array = Enumerable.Range(0, Max).ToArray();
            CollectionAssert.AreEqual(array, builder.ToArray());
        }

        [Test]
        public void AddRangeTest()
        {
            using var builder = new ArrayBuilder<int>(2);
            builder.AddRange(stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, builder.ToArray());
        }

        [Test]
        public void RemoveAtTest()
        {
            using var builder = new ArrayBuilder<char>(16);
            builder.AddRange(stackalloc char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' });

            builder.RemoveAt(0);
            CollectionAssert.AreEqual(new char[] { 'b', 'c', 'd', 'e', 'f', 'g' }, builder.ToArray());
            Assert.AreEqual(6, builder.Count);

            builder.RemoveAt(5);
            CollectionAssert.AreEqual(new char[] { 'b', 'c', 'd', 'e', 'f' }, builder.ToArray());
            Assert.AreEqual(5, builder.Count);

            builder.RemoveAt(2);
            CollectionAssert.AreEqual(new char[] { 'b', 'c', 'e', 'f'}, builder.ToArray());
            Assert.AreEqual(4, builder.Count);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                builder.RemoveAt(-1);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                builder.RemoveAt(4);
            });
        }

        [Test]
        public void RemoveTest()
        {
            using var builder = new ArrayBuilder<char>(16);
            builder.AddRange(stackalloc char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' });

            builder.Remove('a');
            builder.Remove('g');

            Assert.AreEqual(5, builder.Count);
            CollectionAssert.AreEqual(new char[] { 'b', 'c', 'd', 'e', 'f' }, builder.ToArray());
        }

        [Test]
        public void ClearTest()
        {
            using var builder = new ArrayBuilder<char>(16);
            builder.AddRange(stackalloc char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' });

            Assert.AreEqual(7, builder.Count);
            builder.Clear();
            Assert.AreEqual(0, builder.Count);
        }
    }
}
