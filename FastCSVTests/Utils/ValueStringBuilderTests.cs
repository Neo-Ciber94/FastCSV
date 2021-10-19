using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using FastCSV.Utils;

namespace FastCSV.Utils.Tests
{
    [TestFixture()]
    public class ValueStringBuilderTests
    {
        [Test()]
        public void ValueStringBuilderTest()
        {
            Span<char> buffer = stackalloc char[64];

            using ValueStringBuilder sb = new ValueStringBuilder(buffer);

            Assert.AreEqual(64, sb.Capacity);
            Assert.AreEqual(0, sb.Length);
            Assert.True(sb.IsEmpty);
        }

        [Test()]
        public void ValueStringBuilderTest1()
        {
            using ValueStringBuilder sb = new ValueStringBuilder(64);

            Assert.True(sb.Capacity >= 64);
            Assert.AreEqual(0, sb.Length);
            Assert.True(sb.IsEmpty);
        }

        [Test()]
        public void AppendTest()
        {
            Span<char> buffer = stackalloc char[5];

            using ValueStringBuilder sb = new ValueStringBuilder(buffer);

            sb.Append('H');
            sb.Append('e');
            sb.Append('l');
            sb.Append('l');
            sb.Append('o');

            Assert.AreEqual(5, sb.Length);
            Assert.False(sb.IsEmpty);
            Assert.AreEqual ("Hello", sb.ToString());

            sb.Append(' ');
            sb.Append('W');
            sb.Append('o');
            sb.Append('r');
            sb.Append('l');
            sb.Append('d');

            Assert.AreEqual(11, sb.Length);
            Assert.False(sb.IsEmpty);
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [Test()]
        public void AppendTest1()
        {
            Span<char> buffer = stackalloc char[11];

            using ValueStringBuilder sb = new ValueStringBuilder(buffer);
            sb.Append("Hello");
            sb.Append(" ");
            sb.Append("World");

            Assert.AreEqual(11, sb.Capacity);
            Assert.AreEqual(11, sb.Length);
            Assert.False(sb.IsEmpty);
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [Test()]
        public void AppendTest2()
        {
            using ValueStringBuilder sb = new ValueStringBuilder(11);
            sb.Append("Hello");
            sb.Append(" ");
            sb.Append("World");

            Assert.True(sb.Capacity >= 11);
            Assert.AreEqual(11, sb.Length);
            Assert.False(sb.IsEmpty);
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [Test()]
        public void AppendTest4()
        {
            Span<char> buffer = stackalloc char[16];
            using ValueStringBuilder sb = new ValueStringBuilder(buffer);

            ReadOnlySpan<char> str = "Hello World".AsSpan();
            sb.Append(str);

            Assert.True(sb.Capacity >= 16);
            Assert.AreEqual(11, sb.Length);
            Assert.False(sb.IsEmpty);
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [Test()]
        public void AppendTest5()
        {
            Span<char> buffer = stackalloc char[12];
            using ValueStringBuilder sb = new ValueStringBuilder(buffer);

            sb.Append('a', 5);
            sb.Append('b', 5);
            sb.Append('c', 5);

            Assert.AreEqual(15, sb.Length);
            Assert.False(sb.IsEmpty);
            Assert.AreEqual("aaaaabbbbbccccc", sb.ToString());
        }

        [Test()]
        public void AppendLineTest()
        {
            Span<char> buffer = stackalloc char[4];
            using ValueStringBuilder sb = new ValueStringBuilder(buffer);
            sb.AppendLine('H');
            sb.AppendLine('e');
            sb.AppendLine('l');
            sb.AppendLine('l');
            sb.AppendLine('o');

            int newLineLength = Environment.NewLine.Length * 5;
            string newLine = Environment.NewLine;
            Assert.AreEqual(5 + newLineLength, sb.Length);
            Assert.AreEqual($"H{newLine}e{newLine}l{newLine}l{newLine}o{newLine}", sb.ToString());
        }

        [Test()]
        public void AppendLineTest1()
        {
            Span<char> buffer = stackalloc char[16];
            using ValueStringBuilder sb = new ValueStringBuilder(buffer);
            sb.AppendLine("Hello");
            sb.AppendLine("World");

            int newLineLength = Environment.NewLine.Length * 2;
            string newLine = Environment.NewLine;

            Assert.AreEqual(10 + newLineLength, sb.Length);
            Assert.AreEqual($"Hello{newLine}World{newLine}", sb.ToString());
        }

        [Test()]
        public void AppendLineTest2()
        {
            Span<char> buffer = stackalloc char[16];
            using ValueStringBuilder sb = new ValueStringBuilder(buffer);
            sb.Append("Hello");
            sb.AppendLine();
            sb.Append("World");
            sb.AppendLine();

            int newLineLength = Environment.NewLine.Length * 2;
            Assert.AreEqual(10 + newLineLength, sb.Length);
            Assert.AreEqual($"Hello{Environment.NewLine}World{Environment.NewLine}", sb.ToString());
        }

        [Test()]
        public void AppendLineTest3()
        {
            Span<char> buffer = stackalloc char[16];
            using ValueStringBuilder sb = new ValueStringBuilder(buffer);
            sb.AppendLine("Hello".AsSpan());
            sb.AppendLine("World".AsSpan());

            int newLineLength = Environment.NewLine.Length * 2;
            Assert.AreEqual(10 + newLineLength, sb.Length);
            Assert.AreEqual($"Hello{Environment.NewLine}World{Environment.NewLine}", sb.ToString());
        }

        [Test()]
        public void AppendJoinTest1()
        {
            using var sb = new ValueStringBuilder(stackalloc char[64]);
            sb.AppendJoin(',', new int[] { 1, 2, 3, 4 });

            Assert.AreEqual("1,2,3,4", sb.ToString());
        }

        [Test()]
        public void AppendJoinTest2()
        {
            using var sb = new ValueStringBuilder(stackalloc char[64]);
            sb.AppendJoin(", ", new int[] { 1, 2, 3, 4 });

            Assert.AreEqual("1, 2, 3, 4", sb.ToString());
        }

        [Test()]
        public void AppendJoinTest3()
        {
            using var sb = new ValueStringBuilder(stackalloc char[64]);
            ReadOnlySpan<int> values = new int[] { 1, 2, 3, 4 };
            sb.AppendJoin(',', values);

            Assert.AreEqual("1,2,3,4", sb.ToString());
        }

        [Test()]
        public void AppendJoinTest4()
        {
            using var sb = new ValueStringBuilder(stackalloc char[64]);
            ReadOnlySpan<int> values = new int[] { 1, 2, 3, 4 };
            sb.AppendJoin(", ", values);

            Assert.AreEqual("1, 2, 3, 4", sb.ToString());
        }

        [Test()]
        public void AppendJoinTest5()
        {
            using var sb = new ValueStringBuilder(stackalloc char[64]);
            ReadOnlySpan<int> values = new int[] { 1, 2, 3, 4 };
            ReadOnlySpan<char> separator = ", ".AsSpan();
            sb.AppendJoin(separator, values);

            Assert.AreEqual("1, 2, 3, 4", sb.ToString());
        }

        [Test()]
        public void InsertTest()
        {
            using ValueStringBuilder sb = ValueStringBuilder.CreateFrom("HelloWorld".AsSpan());
            Assert.AreEqual("HelloWorld", sb.ToString());

            sb.Insert(5, ' ');
            Assert.AreEqual(11, sb.Length);
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [Test()]
        public void InsertTest1()
        {
            using ValueStringBuilder sb = ValueStringBuilder.CreateFrom("Hello".AsSpan());
            sb.Insert(5, " World");
            Assert.AreEqual(11, sb.Length);
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [Test()]
        public void InsertTest2()
        {
            using ValueStringBuilder sb = ValueStringBuilder.CreateFrom("Hello".AsSpan());
            sb.Insert(5, " World".AsSpan());
            Assert.AreEqual(11, sb.Length);
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [Test()]
        public void InsertTest3()
        {
            using ValueStringBuilder sb = ValueStringBuilder.CreateFrom("llo ".AsSpan());
            sb.Insert(0, "He".AsSpan());
            sb.Insert(6, "World".AsSpan());
            Assert.AreEqual(11, sb.Length);
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [Test()]
        public void ClearTest()
        {
            using ValueStringBuilder sb = ValueStringBuilder.CreateFrom("Hello World".AsSpan());
            Assert.AreEqual("Hello World", sb.ToString());

            sb.Clear();

            Assert.AreEqual(0, sb.Length);
            Assert.AreEqual(string.Empty, sb.ToString());
        }

        [Test()]
        public void ToStringTest()
        {
            Span<char> buffer = stackalloc char[4];
            using ValueStringBuilder sb = new ValueStringBuilder(buffer);

            Assert.AreEqual(string.Empty, sb.ToString());
        }

        [Test()]
        public void DisposeTest()
        {
            ValueStringBuilder sb = ValueStringBuilder.CreateFrom("Hello World".AsSpan());
            Assert.AreEqual(11, sb.Length);
            Assert.AreEqual("Hello World", sb.ToString());

            sb.Dispose();
            Assert.True(sb.IsEmpty);
            Assert.AreEqual(0, sb.Length);
            Assert.AreEqual(0, sb.Capacity);
        }

        [Test()]
        public void ToStringAndDisposeTest()
        {
            ValueStringBuilder sb = ValueStringBuilder.CreateFrom("Hello World".AsSpan());

            Assert.AreEqual("Hello World", sb.ToStringAndDispose());
            Assert.True(sb.IsEmpty);
            Assert.AreEqual(0, sb.Length);
            Assert.AreEqual(0, sb.Capacity);
        }

        [Test()]
        public void EnsureCapacityTest()
        {
            Span<char> buffer = stackalloc char[4];
            using ValueStringBuilder sb = new ValueStringBuilder(buffer);

            sb.EnsureCapacity(32);
            Assert.True(sb.Capacity >= 32);
            Assert.AreEqual(0, sb.Length);
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            using ValueStringBuilder sb = ValueStringBuilder.CreateFrom("Hello".AsSpan());

            var enumerator = sb.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.AreEqual('H', enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual('e', enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual('l', enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual('l', enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.AreEqual('o', enumerator.Current);

            Assert.False(enumerator.MoveNext());
        }
    }
}