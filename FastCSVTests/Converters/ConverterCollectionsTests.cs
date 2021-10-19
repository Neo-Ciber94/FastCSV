using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters
{
    [TestFixture]
    public class ConverterCollectionsTests
    {
        [Test]
        public void TwoCollectionsTests()
        {
            var options = new CsvConverterOptions
            {
                CollectionHandling = CollectionHandling.Default
            };

            var values = new TwoCollections<int, string>(
                new[] { 1, 2, 3 },
                new[] { "red", "blue" }
            );

            string deserialized = CsvConverter.Serialize(values, options);
            Assert.AreEqual($"item1,item2,item3,item1,item2{System.Environment.NewLine}1,2,3,red,blue", deserialized);

            var serialized = CsvConverter.Deserialize<TwoCollections<int, string>>(deserialized, options);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, serialized.First);
            CollectionAssert.AreEqual(new[] { "red", "blue" }, serialized.Second);
        }

        [Test]
        public void ThreeCollectionsTests()
        {
            var options = new CsvConverterOptions
            {
                CollectionHandling = CollectionHandling.Default
            };

            var values = new ThreeCollections<int, string, char>(
                new[] { 1, 2, 3 },
                new[] { "red", "blue" },
                new[] { 'a', 'b', 'c'}
            );

            string deserialized = CsvConverter.Serialize(values, options);
            Assert.AreEqual($"item1,item2,item3,item1,item2,item1,item2,item3{System.Environment.NewLine}1,2,3,red,blue,a,b,c", deserialized);

            var serialized = CsvConverter.Deserialize<ThreeCollections<int, string, char>>(deserialized, options);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, serialized.First);
            CollectionAssert.AreEqual(new[] { "red", "blue" }, serialized.Second);
            CollectionAssert.AreEqual(new[] { 'a', 'b', 'c' }, serialized.Third);
        }

        [Test]
        public void ThreeCollectionSingleItemTest()
        {
            var options = new CsvConverterOptions
            {
                CollectionHandling = CollectionHandling.Default
            };

            var values = new ThreeCollections<int, int, int>(
                new[] { 1 },
                new[] { 2 },
                new[] { 3 }
            );

            string deserialized = CsvConverter.Serialize(values, options);
            Assert.AreEqual($"item1,item1,item1{System.Environment.NewLine}1,2,3", deserialized);

            var serialized = CsvConverter.Deserialize<ThreeCollections<int, int, int>>(deserialized, options);
            CollectionAssert.AreEqual(new[] { 1 }, serialized.First);
            CollectionAssert.AreEqual(new[] { 2 }, serialized.Second);
            CollectionAssert.AreEqual(new[] { 3 }, serialized.Third);
        }

        [Test]
        public void ThreeCollectionLastEmptyTest()
        {
            var options = new CsvConverterOptions
            {
                CollectionHandling = CollectionHandling.Default
            };

            var values = new ThreeCollections<int, int, int>(
                new[] { 1 },
                new[] { 2 },
                new int[0]
            );

            string deserialized = CsvConverter.Serialize(values, options);
            Assert.AreEqual($"item1,item1{System.Environment.NewLine}1,2", deserialized);

            var serialized = CsvConverter.Deserialize<ThreeCollections<int, int, int>>(deserialized, options);
            CollectionAssert.AreEqual(new[] { 1 }, serialized.First);
            CollectionAssert.AreEqual(new[] { 2 }, serialized.Second);
            CollectionAssert.AreEqual(new int[0], serialized.Third);
        }

        [Test]
        public void ThreeCollectionMiddleEmptyTest()
        {
            var options = new CsvConverterOptions
            {
                CollectionHandling = CollectionHandling.Default
            };

            var values = new ThreeCollections<int, int, int>(
                new[] { 1 },
                new int[0],
                new[] { 3 }
            );

            /*
             This is the actual expected behaviour,
             currently there is not way to know where the collection is located
             */

            string deserialized = CsvConverter.Serialize(values, options);
            Assert.AreEqual($"item1,item1{System.Environment.NewLine}1,3", deserialized);

            var serialized = CsvConverter.Deserialize<ThreeCollections<int, int, int>>(deserialized, options);
            CollectionAssert.AreEqual(new[] { 1 }, serialized.First);
            CollectionAssert.AreEqual(new[] { 3 }, serialized.Second);
            CollectionAssert.AreEqual(new int[0], serialized.Third);
        }

        [Test]
        public void CollectionInvalidIndexTest()
        {
            var options = new CsvConverterOptions
            {
                CollectionHandling = CollectionHandling.Default
            };

            string csv = $"item1,item3,item2{System.Environment.NewLine}1,2,3";

            Assert.Throws<InvalidOperationException>(() =>
            {
                _ = CsvConverter.Deserialize<TwoCollections<int, int>>(csv);
            });
        }

        public record TwoCollections<T1, T2>(T1[] First, T2[] Second);

        public record ThreeCollections<T1, T2, T3>(T1[] First, T2[] Second, T3[] Third);
    }
}
