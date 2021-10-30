using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Utils;
using NUnit.Framework;

namespace FastCSV.Internal.Tests
{
    [TestFixture]
    public class Utf16ReaderTests
    {
        [Test]
        public void FillBufferTest()
        {
            var stream = StreamHelper.CreateStreamFromString("Hello World");
            using var reader = new Utf16Reader(stream);

            var buffer = reader.FillBuffer();
            Assert.AreEqual('H', buffer[0]);
            Assert.AreEqual('e', buffer[1]);
            Assert.AreEqual('l', buffer[2]);
            Assert.AreEqual('l', buffer[3]);
            Assert.AreEqual('o', buffer[4]);
            Assert.AreEqual(' ', buffer[5]);
            Assert.AreEqual('W', buffer[6]);
            Assert.AreEqual('o', buffer[7]);
            Assert.AreEqual('r', buffer[8]);
            Assert.AreEqual('l', buffer[9]);
            Assert.AreEqual('d', buffer[10]);

            Assert.False(reader.IsDone);
        }

        [Test]
        public void ConsumeTest()
        {
            var stream = StreamHelper.CreateStreamFromString("Hello World");
            using var reader = new Utf16Reader(stream);
            var buffer = reader.FillBuffer();
            Assert.AreEqual(11, buffer.Length);
            reader.Consume(6);

            buffer = reader.FillBuffer();
            Assert.AreEqual('W', buffer[0]);
            Assert.AreEqual('o', buffer[1]);
            Assert.AreEqual('r', buffer[2]);
            Assert.AreEqual('l', buffer[3]);
            Assert.AreEqual('d', buffer[4]);
            Assert.False(reader.IsDone);
        }

        [Test]
        public void ReadTest()
        {
            var stream = StreamHelper.CreateStreamFromString("Hello World");
            using var reader = new Utf16Reader(stream);

            Span<char> buffer = stackalloc char[5];
            int written = reader.Read(buffer);

            Assert.AreEqual(5, written);
            Assert.AreEqual(buffer[0], 'H');
            Assert.AreEqual(buffer[1], 'e');
            Assert.AreEqual(buffer[2], 'l');
            Assert.AreEqual(buffer[3], 'l');
            Assert.AreEqual(buffer[4], 'o');

            reader.Consume(1);

            written = reader.Read(buffer);
            Assert.AreEqual(5, written);
            Assert.AreEqual(buffer[0], 'W');
            Assert.AreEqual(buffer[1], 'o');
            Assert.AreEqual(buffer[2], 'r');
            Assert.AreEqual(buffer[3], 'l');
            Assert.AreEqual(buffer[4], 'd');

            Assert.True(reader.IsDone);
        }

        [Test]
        public void ReadNextTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var reader = new Utf16Reader(stream);

            Assert.AreEqual('H', reader.ReadNext());
            Assert.AreEqual('e', reader.ReadNext());
            Assert.AreEqual('l', reader.ReadNext());
            Assert.AreEqual('l', reader.ReadNext());
            Assert.AreEqual('o', reader.ReadNext());

            Assert.AreEqual(' ', reader.ReadNext());
            Assert.AreEqual('W', reader.ReadNext());
            Assert.AreEqual('o', reader.ReadNext());
            Assert.AreEqual('r', reader.ReadNext());
            Assert.AreEqual('l', reader.ReadNext());
            Assert.AreEqual('d', reader.ReadNext());

            Assert.True(reader.IsDone);
            Assert.AreEqual(-1, reader.ReadNext());
        }

        [Test]
        public void PeekTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var reader = new Utf16Reader(stream);

            Assert.AreEqual('H', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual('e', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual('l', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual('l', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual('o', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual(' ', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual('W', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual('o', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual('r', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual('l', (char)reader.Peek());
            reader.Consume(1);

            Assert.AreEqual('d', (char)reader.Peek());
            reader.Consume(1);

            Assert.True(reader.IsDone);
            Assert.AreEqual(-1, reader.ReadNext());
        }

        [Test]
        public void ReadUntilTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var reader = new Utf16Reader(stream);
            char[] data = reader.ReadUntil(' ');

            Assert.AreEqual(5, data.Length);
            Assert.AreEqual('H', (char)data[0]);
            Assert.AreEqual('e', (char)data[1]);
            Assert.AreEqual('l', (char)data[2]);
            Assert.AreEqual('l', (char)data[3]);
            Assert.AreEqual('o', (char)data[4]);

            Assert.False(reader.IsDone);
        }

        [Test]
        public void ReadUntilValueNotFoundTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var reader = new Utf16Reader(stream);
            char[] data = reader.ReadUntil('!');

            Assert.AreEqual(11, data.Length);
            Assert.AreEqual('H', (char)data[0]);
            Assert.AreEqual('e', (char)data[1]);
            Assert.AreEqual('l', (char)data[2]);
            Assert.AreEqual('l', (char)data[3]);
            Assert.AreEqual('o', (char)data[4]);
            Assert.AreEqual(' ', (char)data[5]);
            Assert.AreEqual('W', (char)data[6]);
            Assert.AreEqual('o', (char)data[7]);
            Assert.AreEqual('r', (char)data[8]);
            Assert.AreEqual('l', (char)data[9]);
            Assert.AreEqual('d', (char)data[10]);

            Assert.True(reader.IsDone);
        }

        [Test]
        public void ReadLineTest1()
        {
            var text = "Hello World!,\nAnd good night";
            using var reader = new Utf16Reader(StreamHelper.CreateStreamFromString(text));

            var s1 = reader.ReadLine();
            Assert.AreEqual("Hello World!,", s1);

            var s2 = reader.ReadLine();
            Assert.AreEqual("And good night", s2);

            Assert.True(reader.IsDone);
        }

        [Test]
        public void ReadLineTest2()
        {
            var text = "Hello World!,\r\nAnd good night";
            using var reader = new Utf16Reader(StreamHelper.CreateStreamFromString(text));

            var s1 = reader.ReadLine();
            Assert.AreEqual("Hello World!,", s1);

            var s2 = reader.ReadLine();
            Assert.AreEqual("And good night", s2);

            Assert.True(reader.IsDone);
        }

        [Test]
        public void ReadLineToEndTest()
        {
            var text = "Hello World!,\r\nAnd good night";
            using var reader = new Utf16Reader(StreamHelper.CreateStreamFromString(text));

            string s = reader.ReadToEnd();
            Assert.AreEqual("Hello World!,\r\nAnd good night", s);
            Assert.True(reader.IsDone);
        }

        [Test]
        public void DiscardBufferTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var reader = new Utf16Reader(stream);
            var buffer = reader.FillBuffer();

            Assert.AreEqual(11, buffer.Length);
            Assert.AreEqual(buffer[0], 'H');
            Assert.AreEqual(buffer[1], 'e');
            Assert.AreEqual(buffer[2], 'l');
            Assert.AreEqual(buffer[3], 'l');
            Assert.AreEqual(buffer[4], 'o');
            Assert.AreEqual(buffer[5], ' ');
            Assert.AreEqual(buffer[6], 'W');
            Assert.AreEqual(buffer[7], 'o');
            Assert.AreEqual(buffer[8], 'r');
            Assert.AreEqual(buffer[9], 'l');
            Assert.AreEqual(buffer[10], 'd');
            Assert.False(reader.IsDone);

            reader.DiscardBuffer();

            buffer = reader.FillBuffer();

            Assert.AreEqual(0, buffer.Length);
            Assert.True(reader.IsDone);
        }

        [Test]
        public void ThrowIfDisposedTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);
            var reader = new Utf16Reader(stream);

            Assert.True(stream.CanRead);
            Assert.False(reader.IsDisposed);

            reader.Dispose();

            Assert.False(stream.CanRead);
            Assert.True(reader.IsDisposed);

            Assert.Throws<ObjectDisposedException>(() =>
            {
                char[] buffer = new char[10];
                reader.Read(buffer);
            });
        }
    }
}
