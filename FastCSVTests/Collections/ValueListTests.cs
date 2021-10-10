using NUnit.Framework;
using System;

namespace FastCSV.Collections.Tests
{
    [TestFixture]
    public class ValueListTests
    {
        [Test()]
        public void ValueListTest()
        {
            using ValueList<int> list = new ValueList<int>(stackalloc int[4]);
            Assert.AreEqual(0, list.Length);
            Assert.AreEqual(4, list.Capacity);
            Assert.True(list.IsEmpty);
        }

        [Test()]
        public void ValueListTest1()
        {
            using ValueList<int> list = new ValueList<int>(6);
            Assert.AreEqual(0, list.Length);
            Assert.True(list.IsEmpty);
        }

        [Test()]
        public void CreateFromTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(5, list.Length);
            Assert.False(list.IsEmpty);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            Assert.AreEqual(4, list[3]);
            Assert.AreEqual(5, list[4]);
        }

        [Test()]
        public void SpanTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3, 4 });
            ReadOnlySpan<int> span = list.Span;

            Assert.AreEqual(4, span.Length);
            Assert.AreEqual(1, span[0]);
            Assert.AreEqual(2, span[1]);
            Assert.AreEqual(3, span[2]);
            Assert.AreEqual(4, span[3]);

            list.Clear();
            span = list.Span;
            Assert.AreEqual(0, span.Length);
        }

        [Test()]
        public void AddTest()
        {
            using ValueList<int> list = new ValueList<int>(stackalloc int[4]);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            {
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
                Assert.AreEqual(4, list[3]);
            }

            Assert.AreEqual(4, list.Length);
            Assert.False(list.IsEmpty);

            list.Add(5);
            list.Add(6);

            {
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
                Assert.AreEqual(4, list[3]);
                Assert.AreEqual(5, list[4]);
                Assert.AreEqual(6, list[5]);
            }

            Assert.AreEqual(6, list.Length);
            Assert.False(list.IsEmpty);
        }

        [Test()]
        public void AddRangeTest()
        {
            using ValueList<int> list = new ValueList<int>(stackalloc int[4]);
            list.AddRange(stackalloc int[] { 1, 2, 3, 4 });

            {
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
                Assert.AreEqual(4, list[3]);
            }

            Assert.AreEqual(4, list.Length);
            Assert.False(list.IsEmpty);

            list.AddRange(stackalloc int[] { 5, 6 });

            {
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
                Assert.AreEqual(4, list[3]);
                Assert.AreEqual(5, list[4]);
                Assert.AreEqual(6, list[5]);
            }

            Assert.AreEqual(6, list.Length);
            Assert.False(list.IsEmpty);
        }

        [Test()]
        public void InsertTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3, 5 });
            list.Insert(3, 4);
            list.Insert(0, 0);

            Assert.AreEqual(6, list.Length);
            Assert.False(list.IsEmpty);

            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
            Assert.AreEqual(3, list[3]);
            Assert.AreEqual(4, list[4]);
            Assert.AreEqual(5, list[5]);
        }

        [Test()]
        public void InsertRangeTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] {  2, 3 });
            list.InsertRange(2, stackalloc int[] { 4, 5 });
            list.InsertRange(0, stackalloc int[] { 0, 1 });

            Assert.AreEqual(6, list.Length);
            Assert.False(list.IsEmpty);
        }

        [Test()]
        public void RemoveLastTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(5, list.RemoveLast());
            Assert.AreEqual(4, list.RemoveLast());
            Assert.AreEqual(3, list.RemoveLast());
            Assert.AreEqual(2, list.RemoveLast());
            Assert.AreEqual(1, list.RemoveLast());
            Assert.True(list.IsEmpty);

            Exception exception = null;

            try
            {
                list.RemoveLast();
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }

            Assert.NotNull(exception);
        }

        [Test()]
        public void RemoveTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3 });

            Assert.True(list.Remove(3));
            Assert.AreEqual(2, list.Length);

            Assert.True(list.Remove(2));
            Assert.AreEqual(1, list.Length);

            Assert.True(list.Remove(1));
            Assert.AreEqual(0, list.Length);

            Assert.False(list.Remove(3));
        }

        [Test()]
        public void RemoveAtTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3, 4, 5 });

            list.RemoveAt(2);
            Assert.AreEqual(new int[] { 1, 2, 4, 5 }, list.ToArray());

            list.RemoveAt(0);
            Assert.AreEqual(new int[] { 2, 4, 5 }, list.ToArray());

            list.RemoveAt(2);
            Assert.AreEqual(new int[] { 2, 4 }, list.ToArray());

            list.RemoveAt(1);
            Assert.AreEqual(new int[] { 2 }, list.ToArray());

            list.RemoveAt(0);
            Assert.AreEqual(Array.Empty<int>(), list.ToArray());

            Exception exception = null;

            try
            {
                list.RemoveAt(1);
            }
            catch(ArgumentOutOfRangeException e)
            {
                exception = e;
            }

            Assert.NotNull(exception);
        }

        [Test()]
        public void ClearTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3, 4 });
            list.Clear();

            Assert.AreEqual(0, list.Length);
        }

        [Test()]
        public void IndexOfTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3, 4 });
            Assert.AreEqual(0, list.IndexOf(1));
            Assert.AreEqual(1, list.IndexOf(2));
            Assert.AreEqual(2, list.IndexOf(3));
            Assert.AreEqual(3, list.IndexOf(4));

            Assert.AreEqual(-1, list.IndexOf(0));
            Assert.AreEqual(-1, list.IndexOf(5));
        }

        [Test()]
        public void ContainsTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3, 4 });
            Assert.True(list.Contains(1));
            Assert.True(list.Contains(2));
            Assert.True(list.Contains(3));
            Assert.True(list.Contains(4));

            Assert.False(list.Contains(0));
            Assert.False(list.Contains(5));
        }


        [Test()]
        public void LastIndexOfTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3, 4 });
            Assert.AreEqual(0, list.LastIndexOf(1));
            Assert.AreEqual(1, list.LastIndexOf(2));
            Assert.AreEqual(2, list.LastIndexOf(3));
            Assert.AreEqual(3, list.LastIndexOf(4));

            Assert.AreEqual(-1, list.LastIndexOf(0));
            Assert.AreEqual(-1, list.LastIndexOf(5));
        }

        [Test()]
        public void ToArrayTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3 });
            Assert.AreEqual(new int[] { 1, 2, 3 }, list.ToArray());
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3 });

            var enumerator = list.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);

            Assert.False(enumerator.MoveNext());
        }

        [Test()]
        public void ToStringTest()
        {
            using ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3 });
            Assert.AreEqual("[1, 2, 3]", list.ToString());

            list.Clear();
            Assert.AreEqual("[]", list.ToString());
        }

        [Test()]
        public void DisposeTest()
        {
            ValueList<int> list = ValueList<int>.CreateFrom(stackalloc int[] { 1, 2, 3 });
            list.Dispose();

            Assert.AreEqual(0, list.Length);
            Assert.AreEqual(0, list.Capacity);
            Assert.True(list.IsEmpty);
        }
    }
}