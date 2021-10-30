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
    public class Utf8ReaderTests
    {
        [Test]
        public void FillBufferTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var utf8Reader = new Utf8Reader(stream);
            var buffer = utf8Reader.FillBuffer();

            Assert.AreEqual(11, buffer.Length);
            Assert.AreEqual((char)buffer[0], 'H');
            Assert.AreEqual((char)buffer[1], 'e');
            Assert.AreEqual((char)buffer[2], 'l');
            Assert.AreEqual((char)buffer[3], 'l');
            Assert.AreEqual((char)buffer[4], 'o');
            Assert.AreEqual((char)buffer[5], ' ');
            Assert.AreEqual((char)buffer[6], 'W');
            Assert.AreEqual((char)buffer[7], 'o');
            Assert.AreEqual((char)buffer[8], 'r');
            Assert.AreEqual((char)buffer[9], 'l');
            Assert.AreEqual((char)buffer[10], 'd');

            Assert.False(utf8Reader.IsDone);
        }

        [Test]
        public void ConsumeTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var utf8Reader = new Utf8Reader(stream);
            var buffer = utf8Reader.FillBuffer();

            Assert.AreEqual(11, buffer.Length);
            utf8Reader.Consume(6);

            var newBuffer = utf8Reader.FillBuffer();

            Assert.AreEqual(5, newBuffer.Length);
            Assert.AreEqual((char)newBuffer[0], 'W');
            Assert.AreEqual((char)newBuffer[1], 'o');
            Assert.AreEqual((char)newBuffer[2], 'r');
            Assert.AreEqual((char)newBuffer[3], 'l');
            Assert.AreEqual((char)newBuffer[4], 'd');

            Assert.False(utf8Reader.IsDone);
        }

        [Test]
        public void ReadTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var utf8Reader = new Utf8Reader(stream);

            Span<byte> buffer = stackalloc byte[5];
            int written = utf8Reader.Read(buffer);

            Assert.AreEqual(5, written);
            Assert.AreEqual((char)buffer[0], 'H');
            Assert.AreEqual((char)buffer[1], 'e');
            Assert.AreEqual((char)buffer[2], 'l');
            Assert.AreEqual((char)buffer[3], 'l');
            Assert.AreEqual((char)buffer[4], 'o');

            utf8Reader.Consume(1);

            written = utf8Reader.Read(buffer);
            Assert.AreEqual(5, written);
            Assert.AreEqual((char)buffer[0], 'W');
            Assert.AreEqual((char)buffer[1], 'o');
            Assert.AreEqual((char)buffer[2], 'r');
            Assert.AreEqual((char)buffer[3], 'l');
            Assert.AreEqual((char)buffer[4], 'd');

            Assert.True(utf8Reader.IsDone);
        }

        [Test]
        public void ReadNextTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var utf8Reader = new Utf8Reader(stream);

            Assert.AreEqual('H', (char)utf8Reader.ReadNext());
            Assert.AreEqual('e', (char)utf8Reader.ReadNext());
            Assert.AreEqual('l', (char)utf8Reader.ReadNext());
            Assert.AreEqual('l', (char)utf8Reader.ReadNext());
            Assert.AreEqual('o', (char)utf8Reader.ReadNext());

            Assert.AreEqual(' ', (char)utf8Reader.ReadNext());
            Assert.AreEqual('W', (char)utf8Reader.ReadNext());
            Assert.AreEqual('o', (char)utf8Reader.ReadNext());
            Assert.AreEqual('r', (char)utf8Reader.ReadNext());
            Assert.AreEqual('l', (char)utf8Reader.ReadNext());
            Assert.AreEqual('d', (char)utf8Reader.ReadNext());

            Assert.True(utf8Reader.IsDone);
            Assert.AreEqual(-1, utf8Reader.ReadNext());
        }

        [Test]
        public void PeekTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var utf8Reader = new Utf8Reader(stream);

            Assert.AreEqual('H', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual('e', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual('l', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual('l', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual('o', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual(' ', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual('W', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual('o', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual('r', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual('l', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.AreEqual('d', (char)utf8Reader.Peek());
            utf8Reader.Consume(1);

            Assert.True(utf8Reader.IsDone);
            Assert.AreEqual(-1, utf8Reader.ReadNext());
        }

        [Test]
        public void ReadUntilTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var utf8Reader = new Utf8Reader(stream);
            byte[] data = utf8Reader.ReadUntil((byte)' ');

            Assert.AreEqual(5, data.Length);
            Assert.AreEqual('H', (char)data[0]);
            Assert.AreEqual('e', (char)data[1]);
            Assert.AreEqual('l', (char)data[2]);
            Assert.AreEqual('l', (char)data[3]);
            Assert.AreEqual('o', (char)data[4]);

            Assert.False(utf8Reader.IsDone);

            data = utf8Reader.ReadUntil((byte)' ');
            Assert.AreEqual(5, data.Length);
            Assert.AreEqual('W', (char)data[0]);
            Assert.AreEqual('o', (char)data[1]);
            Assert.AreEqual('r', (char)data[2]);
            Assert.AreEqual('l', (char)data[3]);
            Assert.AreEqual('d', (char)data[4]);

            Assert.True(utf8Reader.IsDone);
        }

        [Test]
        public void ReadUntilValueNotFoundTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var utf8Reader = new Utf8Reader(stream);
            byte[] data = utf8Reader.ReadUntil((byte)'!');

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

            Assert.True(utf8Reader.IsDone);
        }


        [Test]
        public void DiscardBufferTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);

            using var utf8Reader = new Utf8Reader(stream);
            var buffer = utf8Reader.FillBuffer();

            Assert.AreEqual(11, buffer.Length);
            Assert.AreEqual((char)buffer[0], 'H');
            Assert.AreEqual((char)buffer[1], 'e');
            Assert.AreEqual((char)buffer[2], 'l');
            Assert.AreEqual((char)buffer[3], 'l');
            Assert.AreEqual((char)buffer[4], 'o');
            Assert.AreEqual((char)buffer[5], ' ');
            Assert.AreEqual((char)buffer[6], 'W');
            Assert.AreEqual((char)buffer[7], 'o');
            Assert.AreEqual((char)buffer[8], 'r');
            Assert.AreEqual((char)buffer[9], 'l');
            Assert.AreEqual((char)buffer[10], 'd');
            Assert.False(utf8Reader.IsDone);

            utf8Reader.DiscardBuffer();

            buffer = utf8Reader.FillBuffer();

            Assert.AreEqual(0, buffer.Length);
            Assert.True(utf8Reader.IsDone);
        }

        [Test]
        public void ThrowIfDisposedTest()
        {
            var text = "Hello World";
            var stream = StreamHelper.CreateStreamFromString(text);
            var utf8Reader = new Utf8Reader(stream);

            Assert.True(stream.CanRead);
            Assert.False(utf8Reader.IsDisposed);

            utf8Reader.Dispose();

            Assert.False(stream.CanRead);
            Assert.True(utf8Reader.IsDisposed);

            Assert.Throws<ObjectDisposedException>(() =>
            {
                byte[] buffer = new byte[10];
                utf8Reader.Read(buffer);
            });
        }
    }
}
