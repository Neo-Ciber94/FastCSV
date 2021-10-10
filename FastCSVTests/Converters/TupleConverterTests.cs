using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;

namespace FastCSV.Converters.Tests
{
    [TestFixture]
    public class TupleConverterTests
    {
        private static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeTuple1Test()
        {
            var tuple = CreateTuple(new Tuple<int>(12));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1\n12", serialized);
        }

        [Test]
        public void SerializeTuple2Test()
        {
            var tuple = CreateTuple(new Tuple<int, string>(12, "red"));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2\n12,red", serialized);
        }

        [Test]
        public void SerializeTuple3Test()
        {
            var tuple = CreateTuple(new Tuple<int, string, float>(12, "red", 250.24f));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3\n12,red,250.24", serialized);
        }

        [Test]
        public void SerializeTuple4Test()
        {
            var tuple = CreateTuple(new Tuple<int, string, float, char>(12, "red", 250.24f, 'c'));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4\n12,red,250.24,c", serialized);
        }

        [Test]
        public void SerializeTuple5Test()
        {
            var tuple = CreateTuple(new Tuple<int, string, float, char, decimal>(12, "red", 250.24f, 'c', 699.99m));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5\n12,red,250.24,c,699.99", serialized);
        }

        [Test]
        public void SerializeTuple6Test()
        {
            var tuple = CreateTuple(new Tuple<int, string, float, char, decimal, bool>(12, "red", 250.24f, 'c', 699.99m, false));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5,item6\n12,red,250.24,c,699.99,false", serialized);
        }

        [Test]
        public void SerializeTuple7Test()
        {
            var tuple = CreateTuple(new Tuple<int, string, float, char, decimal, bool, long>(12, "red", 250.24f, 'c', 699.99m, false, 9000L));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5,item6,item7\n12,red,250.24,c,699.99,false,9000", serialized);
        }

        [Test]
        public void SerializeTuple8Test()
        {
            var tuple = CreateTuple((new Tuple<int, string, float, char, decimal, bool, long, Tuple<Version>>(12, "red", 250.24f, 'c', 699.99m, false, 9000L, new Tuple<Version>(new Version(1, 5, 134)))));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5,item6,item7,item8\n12,red,250.24,c,699.99,false,9000,1.5.134", serialized);
        }

        [Test]
        public void SerializeTupleLongTest()
        {
            var tuple = CreateTuple(new Tuple<int, string, float, char, decimal, bool, long, Tuple<Version, bool, bool, int, int, int>>(12, "red", 250.24f, 'c', 699.99m, false, 9000L, new Tuple<Version, bool, bool, int, int, int>(new Version(1, 5, 134), false, true, 1, 2, 2)));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5,item6,item7,item8,item9,item10,item11,item12,item13\n12,red,250.24,c,699.99,false,9000,1.5.134,false,true,1,2,2", serialized);
        }

        [Test]
        public void DeserializeTuple1Test()
        {
            var csv = "item1\n12";
            var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int>>>(csv, Options);

            Assert.AreEqual(new Tuple<int>(12), deserialized.Items);
        }

        [Test]
        public void DeserializeTuple2Test()
        {
            var csv = "item1,item2\n12,red";
            var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int, string>>>(csv, Options);

            Assert.AreEqual(new Tuple<int, string>(12, "red"), deserialized.Items);
        }

        [Test]
        public void DeserializeTuple3Test()
        {
            var csv = "item1,item2,item3\n12,red,250.24";
            var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int, string, float>>>(csv, Options);

            Assert.AreEqual(new Tuple<int, string, float>(12, "red", 250.24f), deserialized.Items);
        }

        [Test]
        public void DeserializeTuple4Test()
        {
            var csv = "item1,item2,item3,item4\n12,red,250.24,c";
            var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int, string, float, char>>>(csv, Options);

            Assert.AreEqual(new Tuple<int, string, float, char>(12, "red", 250.24f, 'c'), deserialized.Items);
        }

        [Test]
        public void DeserializeTuple5Test()
        {
            var csv = "item1,item2,item3,item4,item5\n12,red,250.24,c,false";
            var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int, string, float, char, bool>>>(csv, Options);

            Assert.AreEqual(new Tuple<int, string, float, char, bool>(12, "red", 250.24f, 'c', false), deserialized.Items);
        }

        [Test]
        public void DeserializeTuple6Test()
        {
            var csv = "item1,item2,item3,item4,item5,item6\n12,red,250.24,c,false,9000";
            var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int, string, float, char, bool, long>>>(csv, Options);

            Assert.AreEqual(new Tuple<int, string, float, char, bool, long>(12, "red", 250.24f, 'c', false, 9000L), deserialized.Items);
        }

        [Test]
        public void DeserializeTuple7Test()
        {
            var csv = "item1,item2,item3,item4,item5,item6,item7\n12,red,250.24,c,false,9000,1.5.243";
            var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int, string, float, char, bool, long, Version>>>(csv, Options);

            Assert.AreEqual(new Tuple<int, string, float, char, bool, long, Version>(12, "red", 250.24f, 'c', false, 9000L, new Version(1, 5, 243)), deserialized.Items);
        }

        [Test]
        public void DeserializeTuple8Test()
        {
            var csv = "item1,item2,item3,item4,item5,item6,item7,item8\n12,red,250.24,c,false,9000,1.5.243,true";
            var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int, string, float, char, bool, long, Version, Tuple<bool>>>>(csv, Options);
            Assert.AreEqual(new Tuple<int, string, float, char, bool, long, Version, Tuple<bool>>(12, "red", 250.24f, 'c', false, 9000L, new Version(1, 5, 243), new Tuple<bool>(true)), deserialized.Items);
        }

        [Test]
        public void DeserializeTuple8ExceptionTest()
        {
            var csv = "item1,item2,item3,item4,item5,item6,item7,item8\n12,red,250.24,c,false,9000,1.5.243,true";

            Assert.Throws<ArgumentException>(() =>
            {
                var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int, string, float, char, bool, long, Version, bool>>>(csv, Options);
            }, "Object of type 'System.Tuple`8[System.Int32,System.String,System.Single,System.Char,System.Boolean,System.Int64,System.Version,System.Tuple`1[System.Boolean]]' cannot be converted to type 'System.Tuple`8[System.Int32,System.String,System.Single,System.Char,System.Boolean,System.Int64,System.Version,System.Boolean]'");
        }

        [Test]
        public void DeserializeTuple9Test()
        {
            var csv = "item1,item2,item3,item4,item5,item6,item7,item8,item9\n12,red,250.24,c,false,9000,1.5.243,true,green";
            var deserialized = CsvConverter.Deserialize<TupleItems<Tuple<int, string, float, char, bool, long, Version, Tuple<bool, string>>>>(csv, Options);

            Assert.AreEqual(new Tuple<int, string, float, char, bool, long, Version, Tuple<bool, string>>(12, "red", 250.24f, 'c', false, 9000L, new Version(1, 5, 243), new Tuple<bool, string>(true, "green")), deserialized.Items);
        }


        record TupleItems<TTuple>(TTuple Items) where TTuple: ITuple;

        private static TupleItems<TTuple> CreateTuple<TTuple>(TTuple tuple) where TTuple: ITuple
        {
            if (!tuple.GetType().IsClass)
            {
                throw new InvalidOperationException("Expected Tuple");
            }

            return new TupleItems<TTuple>(tuple);
        }
    }
}
