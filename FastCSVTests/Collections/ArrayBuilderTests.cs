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
    }
}
