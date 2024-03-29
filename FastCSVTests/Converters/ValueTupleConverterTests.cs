﻿using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    class ValueTupleConverterTests
    {
        private static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeValueTuple1Test()
        {
            var tuple = CreateTuple(new ValueTuple<int>(12));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual($"item1{System.Environment.NewLine}12", serialized);
        }

        [Test]
        public void SerializeValueTuple2Test()
        {
            var tuple = CreateTuple((12, "red"));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual($"item1,item2{System.Environment.NewLine}12,red", serialized);
        }

        [Test]
        public void SerializeValueTuple3Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual($"item1,item2,item3{System.Environment.NewLine}12,red,250.24", serialized);
        }

        [Test]
        public void SerializeValueTuple4Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c'));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual($"item1,item2,item3,item4{System.Environment.NewLine}12,red,250.24,c", serialized);
        }

        [Test]
        public void SerializeValueTuple5Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual($"item1,item2,item3,item4,item5{System.Environment.NewLine}12,red,250.24,c,699.99", serialized);
        }

        [Test]
        public void SerializeValueTuple6Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m, false));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual($"item1,item2,item3,item4,item5,item6{System.Environment.NewLine}12,red,250.24,c,699.99,false", serialized);
        }

        [Test]
        public void SerializeValueTuple7Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m, false, 9000L));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual($"item1,item2,item3,item4,item5,item6,item7{System.Environment.NewLine}12,red,250.24,c,699.99,false,9000", serialized);
        }

        [Test]
        public void SerializeValueTuple8Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m, false, 9000L, new Version(1, 5, 134)));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual($"item1,item2,item3,item4,item5,item6,item7,item8{System.Environment.NewLine}12,red,250.24,c,699.99,false,9000,1.5.134", serialized);
        }

        [Test]
        public void SerializeValueTupleLongTest()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m, false, 9000L, new Version(1, 5, 134), false, true, 1, 2, 2));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual($"item1,item2,item3,item4,item5,item6,item7,item8,item9,item10,item11,item12,item13{System.Environment.NewLine}12,red,250.24,c,699.99,false,9000,1.5.134,false,true,1,2,2", serialized);
        }

        [Test]
        public void DeserializeValueTuple1Test()
        {
            var csv = $"item1{System.Environment.NewLine}12";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<ValueTuple<int>>>(csv, Options);

            Assert.AreEqual(new ValueTuple<int>(12), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple2Test()
        {
            var csv = $"item1,item2{System.Environment.NewLine}12,red";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<ValueTuple<int, string>>>(csv, Options);

            Assert.AreEqual((12, "red"), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple3Test()
        {
            var csv = $"item1,item2,item3{System.Environment.NewLine}12,red,250.24";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<ValueTuple<int, string, float>>>(csv, Options);

            Assert.AreEqual((12, "red", 250.24f), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple4Test()
        {
            var csv = $"item1,item2,item3,item4{System.Environment.NewLine}12,red,250.24,c";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<ValueTuple<int, string, float, char>>>(csv, Options);

            Assert.AreEqual((12, "red", 250.24f, 'c'), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple5Test()
        {
            var csv = $"item1,item2,item3,item4,item5{System.Environment.NewLine}12,red,250.24,c,false";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<ValueTuple<int, string, float, char, bool>>>(csv, Options);

            Assert.AreEqual((12, "red", 250.24f, 'c', false), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple6Test()
        {
            var csv = $"item1,item2,item3,item4,item5,item6{System.Environment.NewLine}12,red,250.24,c,false,9000";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<ValueTuple<int, string, float, char, bool, long>>>(csv, Options);

            Assert.AreEqual((12, "red", 250.24f, 'c', false, 9000L), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple7Test()
        {
            var csv = $"item1,item2,item3,item4,item5,item6,item7{System.Environment.NewLine}12,red,250.24,c,false,9000,1.5.243";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<ValueTuple<int, string, float, char, bool, long, Version>>>(csv, Options);

            Assert.AreEqual((12, "red", 250.24f, 'c', false, 9000L, new Version(1, 5, 243)), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple8Test()
        {
            var csv = $"item1,item2,item3,item4,item5,item6,item7,item8{System.Environment.NewLine}12,red,250.24,c,false,9000,1.5.243,true";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<(int, string, float, char, bool, long, Version, bool)>>(csv, Options);
            Assert.AreEqual((12, "red", 250.24f, 'c', false, 9000L, new Version(1, 5, 243), true), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple8ExceptionTest()
        {
            var csv = $"item1,item2,item3,item4,item5,item6,item7,item8{System.Environment.NewLine}12,red,250.24,c,false,9000,1.5.243,true";

            Assert.Throws<ArgumentException>(() =>
            {
                var deserialized = CsvConverter.Deserialize<ValueTupleItems<ValueTuple<int, string, float, char, bool, long, Version, bool>>>(csv, Options);
            }, "Object of type 'System.ValueTuple`8[System.Int32,System.String,System.Single,System.Char,System.Boolean,System.Int64,System.Version,System.ValueTuple`1[System.Boolean]]' cannot be converted to type 'System.ValueTuple`8[System.Int32,System.String,System.Single,System.Char,System.Boolean,System.Int64,System.Version,System.Boolean]'");
        }

        [Test]
        public void DeserializeValueTuple9Test()
        {
            var csv = $"item1,item2,item3,item4,item5,item6,item7,item8,item9{System.Environment.NewLine}12,red,250.24,c,false,9000,1.5.243,true,green";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<(int, string, float, char, bool, long, Version, bool, string)>>(csv, Options);

            Assert.AreEqual((12, "red", 250.24f, 'c', false, 9000L, new Version(1, 5, 243), true, "green"), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple17Test()
        {
            var csv = $"item1,item2,item3,item4,item5,item6,item7,item8,item9,item10,item11,item12,item13,item14,item15,item16,item17{System.Environment.NewLine}1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,false";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<(int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, bool)>>(csv, Options);

            Assert.AreEqual((1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, false), deserialized.Items);
        }

        [Test]
        public void DeserializeValueTuple18Test()
        {
            var csv = $"item1,item2,item3,item4,item5,item6,item7,item8,item9,item10,item11,item12,item13,item14,item15,item16,item17,item18{System.Environment.NewLine}1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,true,false";
            var deserialized = CsvConverter.Deserialize<ValueTupleItems<(int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, bool, bool)>>(csv, Options);

            Assert.AreEqual((1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, true, false), deserialized.Items);
        }

        record ValueTupleItems<TTuple>(TTuple Items) where TTuple: ITuple;

        private static ValueTupleItems<TTuple> CreateTuple<TTuple>(TTuple tuple) where TTuple: ITuple
        {
            return new ValueTupleItems<TTuple>(tuple);
        }
    }
}
