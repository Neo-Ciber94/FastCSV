﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Utils;
using NUnit.Framework;

namespace FastCSV.Extensions.Tests
{
    [TestFixture]
    public class StringBuilderExtensionsTests
    {
        [Test]
        public void TrimStartTest()
        {
            Assert.AreEqual("Hello", new StringBuilder("  Hello").TrimStart().ToString());
            Assert.AreEqual("Hello", new StringBuilder("Hello").TrimStart().ToString());
            Assert.AreEqual("", new StringBuilder("    ").TrimStart().ToString());
        }

        [Test]
        public void TrimEndTest()
        {
            Assert.AreEqual("World", new StringBuilder("World  ").TrimEnd().ToString());
            Assert.AreEqual("World", new StringBuilder("World").TrimEnd().ToString());
            Assert.AreEqual("", new StringBuilder("     ").TrimEnd().ToString());
        }

        [Test]
        public void TrimTest()
        {
            Assert.AreEqual("Hello World", new StringBuilder("  Hello World ").Trim().ToString());
            Assert.AreEqual("Hello World", new StringBuilder("Hello World").Trim().ToString());
            Assert.AreEqual("", new StringBuilder("     ").Trim().ToString());
        }

        [Test]
        public void SliceTest()
        {
            Assert.AreEqual("Hello", new StringBuilder("Hello World").Slice(0, 5).ToString());
            Assert.AreEqual("World", new StringBuilder("Hello World").Slice(6, 5).ToString());

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new StringBuilder("Hello World").Slice(5, 10);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new StringBuilder("Hello World").Slice(0, 20);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new StringBuilder("Hello World").Slice(10, -2);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new StringBuilder("Hello World").Slice(-5, 4);
            });
        }

        [Test]
        public void SliceStartIndexTest()
        {
            Assert.AreEqual("World", new StringBuilder("Hello World").Slice(6).ToString());
            Assert.AreEqual("", new StringBuilder("Hello World").Slice(11).ToString());
        }

        [Test]
        public void SliceRangeTest()
        {
            Assert.AreEqual("Hello", new StringBuilder("Hello World").Slice(0..5).ToString());
            Assert.AreEqual("World", new StringBuilder("Hello World").Slice(6..11).ToString());
        }

        [Test]
        public void SliceIndexTest()
        {
            Assert.AreEqual("World", new StringBuilder("Hello World").Slice(^5).ToString());
            Assert.AreEqual("Hello World", new StringBuilder("Hello World").Slice(^11).ToString());
        }

        [Test]
        public void PadLeftTest()
        {
            Assert.AreEqual("**Hello", new StringBuilder("Hello").PadLeft("**").ToString()); 
        }

        [Test]
        public void PadRightTest()
        {
            Assert.AreEqual("Hello**", new StringBuilder("Hello").PadRight("**").ToString());
        }

        [Test]
        public void IndexOfTest()
        {
            Assert.AreEqual(0, new StringBuilder("Hello World").IndexOf("Hello"));
            Assert.AreEqual(6, new StringBuilder("Hello World").IndexOf("World"));
            Assert.AreEqual(-1, new StringBuilder("Hello World").IndexOf("Hello!"));
            Assert.AreEqual(-1, new StringBuilder("Hello World").IndexOf("World!"));

            Assert.AreEqual(10, new StringBuilder("Hello World").IndexOf("d"));
            Assert.AreEqual(0, new StringBuilder("Hello World").IndexOf("H"));
        }

        [Test]
        public void LastIndexOfTest()
        {
            Assert.AreEqual(0, new StringBuilder("Hello World").LastIndexOf("Hello"));
            Assert.AreEqual(6, new StringBuilder("Hello World").LastIndexOf("World"));
            Assert.AreEqual(-1, new StringBuilder("Hello World").LastIndexOf("Hello!"));
            Assert.AreEqual(-1, new StringBuilder("Hello World").LastIndexOf("World!"));

            Assert.AreEqual(10, new StringBuilder("Hello World").LastIndexOf("d"));
            Assert.AreEqual(0, new StringBuilder("Hello World").LastIndexOf("H"));
        }

        [Test]
        public void ContainsTest()
        {
            Assert.True(new StringBuilder("Hello World").Contains("Hello"));
            Assert.True(new StringBuilder("Hello World").Contains("World"));
            Assert.False(new StringBuilder("Hello World").Contains("Hello!"));
            Assert.False(new StringBuilder("Hello World").Contains("World!"));
        }

        [Test]
        public void StartsWithTest()
        {
            Assert.True(new StringBuilder("Hello World").StartsWith("Hello"));
            Assert.True(new StringBuilder("Hello World").StartsWith("Hello World"));
            Assert.False(new StringBuilder("Hello World").StartsWith("Hello!"));
            Assert.False(new StringBuilder("Hello World").StartsWith(""));
        }


        [Test]
        public void EndsWithTest()
        {
            Assert.True(new StringBuilder("Hello World").EndsWith("World"));
            Assert.True(new StringBuilder("Hello World").EndsWith("Hello World"));
            Assert.False(new StringBuilder("Hello World").EndsWith("World!"));
            Assert.False(new StringBuilder("Hello World").EndsWith(""));
        }

        [Test]
        public void TrimStartOnceTest()
        {
            Assert.AreEqual("Hello World", new StringBuilder("HHello World").TrimStartOnce("H").ToString());
            Assert.AreEqual("ello World", new StringBuilder("Hello World").TrimStartOnce("H").ToString());
            Assert.AreEqual("llo World", new StringBuilder("ello World").TrimStartOnce("e").ToString());
            Assert.AreEqual("lo World", new StringBuilder("llo World").TrimStartOnce("l").ToString());
            Assert.AreEqual("o World", new StringBuilder("lo World").TrimStartOnce("l").ToString());
            Assert.AreEqual(" World", new StringBuilder("o World").TrimStartOnce("o").ToString());
            Assert.AreEqual("World", new StringBuilder(" World").TrimStartOnce(" ").ToString());
            Assert.AreEqual("orld", new StringBuilder("World").TrimStartOnce("W").ToString());
            Assert.AreEqual("rld", new StringBuilder("orld").TrimStartOnce("o").ToString());
            Assert.AreEqual("ld", new StringBuilder("rld").TrimStartOnce("r").ToString());
            Assert.AreEqual("d", new StringBuilder("ld").TrimStartOnce("l").ToString());
            Assert.AreEqual("", new StringBuilder("d").TrimStartOnce("d").ToString());
        }

        [Test]
        public void TrimEndOnceTest()
        {
            Assert.AreEqual("Hello World", new StringBuilder("Hello Worldd").TrimEndOnce("d").ToString());
            Assert.AreEqual("Hello Worl", new StringBuilder("Hello World").TrimEndOnce("d").ToString());
            Assert.AreEqual("Hello Wor", new StringBuilder("Hello Worl").TrimEndOnce("l").ToString());
            Assert.AreEqual("Hello Wo", new StringBuilder("Hello Wor").TrimEndOnce("r").ToString());
            Assert.AreEqual("Hello W", new StringBuilder("Hello Wo").TrimEndOnce("o").ToString());
            Assert.AreEqual("Hello ", new StringBuilder("Hello W").TrimEndOnce("W").ToString());
            Assert.AreEqual("Hello", new StringBuilder("Hello ").TrimEndOnce(" ").ToString());
            Assert.AreEqual("Hell", new StringBuilder("Hello").TrimEndOnce("o").ToString());
            Assert.AreEqual("Hel", new StringBuilder("Hell").TrimEndOnce("l").ToString());
            Assert.AreEqual("He", new StringBuilder("Hel").TrimEndOnce("l").ToString());
            Assert.AreEqual("H", new StringBuilder("He").TrimEndOnce("e").ToString());
            Assert.AreEqual("", new StringBuilder("H").TrimEndOnce("H").ToString());
        }

        [Test]
        public void TrimOnceTest()
        {
            Assert.AreEqual("Hello", new StringBuilder("**Hello**").TrimOnce("**").ToString());
        }
    }
}
