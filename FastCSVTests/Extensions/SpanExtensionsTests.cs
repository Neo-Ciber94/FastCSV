using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Extensions.Tests
{
    [TestFixture]
    public class SpanExtensionsTests
    {
        [Test]
        public void TryReplaceAll_OldSpan_NewSpan_SameLength_Test()
        {
            ReadOnlySpan<int> values = new int[] { 1, 1, 2, 2, 1, 1, 1, 2 };

            Span<int> buffer = stackalloc int[100];
            values.TryReplaceAll(stackalloc int[] { 1, 1 }, stackalloc int[] { 5, 5 }, buffer, out int written);

            CollectionAssert.AreEqual(new int[] { 5, 5, 2, 2, 5, 5, 1, 2 }, buffer.Slice(0, written).ToArray());
            Assert.AreEqual(8, written);
        }

        [Test]
        public void TryReplaceAll_OldSpan_NewSpan_NewValueLarger_Length_Test()
        {
            ReadOnlySpan<int> values = new int[] { 1, 1, 2, 2, 1, 1, 1, 2 };

            Span<int> buffer = stackalloc int[100];
            values.TryReplaceAll(stackalloc int[] { 1, 1 }, stackalloc int[] { 6, 6, 6 }, buffer, out int written);

            CollectionAssert.AreEqual(new int[] { 6, 6, 6, 2, 2, 6, 6, 6, 1, 2 }, buffer.Slice(0, written).ToArray());
            Assert.AreEqual(10, written);
        }

        [Test]
        public void TryReplaceAll_OldSpan_NewSpan_NewValueLarger_Length_Test2()
        {
            ReadOnlySpan<int> values = new int[] { 1, 1, 2, 2, 1, 1, 1, 2 };

            Span<int> buffer = stackalloc int[100];
            values.TryReplaceAll(stackalloc int[] { 1 }, stackalloc int[] { 6, 6 }, buffer, out int written);

            CollectionAssert.AreEqual(new int[] { 6, 6, 6, 6, 2, 2, 6, 6, 6, 6, 6, 6, 2 }, buffer.Slice(0, written).ToArray());
            Assert.AreEqual(13, written);
        }

        [Test]
        public void TryReplaceAll_OldSpan_NewSpan_OldValueLarger_Length_Test()
        {
            ReadOnlySpan<int> values = new int[] { 1, 1, 2, 2, 1, 1, 1, 2 };

            Span<int> buffer = stackalloc int[100];
            values.TryReplaceAll(stackalloc int[] { 1, 1, 1}, stackalloc int[] { 6 }, buffer, out int written);

            CollectionAssert.AreEqual(new int[] { 1, 1, 2, 2, 6, 2 }, buffer.Slice(0, written).ToArray());
            Assert.AreEqual(6, written);
        }

        [Test]
        public void TryReplaceAll_Empty_Buffer_Test()
        {
            ReadOnlySpan<int> values = new int[] { 1, 1, 2, 2, 1, 1, 1, 2 };

            Span<int> buffer = Array.Empty<int>();
            values.TryReplaceAll(stackalloc int[] { 1, 1 }, stackalloc int[] { 6, 6, 6 }, buffer, out int written);

            CollectionAssert.AreEqual(Array.Empty<int>(), buffer.Slice(0, written).ToArray());
            Assert.AreEqual(0, written);
        }

        [Test]
        public void TryReplaceAll_Buffer_To_Small_Test()
        {
            ReadOnlySpan<int> values = new int[] { 1, 1, 2, 2, 1, 1, 1, 2 };

            Span<int> buffer = new int[5];
            values.TryReplaceAll(stackalloc int[] { 1, 1 }, stackalloc int[] { 6, 6, 6 }, buffer, out int written);

            CollectionAssert.AreEqual(new int[] { 6, 6, 6, 2, 2 }, buffer.Slice(0, written).ToArray());
            Assert.AreEqual(5, written);
        }

        [Test]
        public void TryReplaceAll_With_Empty_Test()
        {
            ReadOnlySpan<int> values = new int[] { 1, 1, 2, 2, 1, 1, 1, 2 };

            Span<int> buffer = new int[100];
            values.TryReplaceAll(stackalloc int[] { 1, 1 }, Array.Empty<int>(), buffer, out int written);

            CollectionAssert.AreEqual(new int[] { 2, 2, 1, 2 }, buffer.Slice(0, written).ToArray());
            Assert.AreEqual(4, written);
        }

        [Test]
        public void TryReplaceAll_Empty_With_Value_Test()
        {
            ReadOnlySpan<int> values = new int[] { 1, 1, 2, 2, 1, 1, 1, 2 };

            Span<int> buffer = new int[100];
            Assert.False(values.TryReplaceAll(Array.Empty<int>(), new int[] { 9, 9 }, buffer, out int written));

            CollectionAssert.AreEqual(Array.Empty<int>(), buffer.Slice(0, written).ToArray());
            Assert.AreEqual(0, written);
        }
    }
}
