using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters
{
    [TestFixture]
    class TupleConverterTests
    {
        private static readonly CsvConverterOptions Options = new CsvConverterOptions { CollectionHandling = CollectionHandling.Default };

        [Test]
        public void SerializeValueTuple1Test()
        {
            var tuple = CreateTuple(new ValueTuple<int>(12));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1\n12", serialized);
        }

        [Test]
        public void SerializeValueTuple2Test()
        {
            var tuple = CreateTuple((12, "red"));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2\n12,red", serialized);
        }

        [Test]
        public void SerializeValueTuple3Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3\n12,red,250.24", serialized);
        }

        [Test]
        public void SerializeValueTuple4Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c'));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4\n12,red,250.24,c", serialized);
        }

        [Test]
        public void SerializeValueTuple5Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5\n12,red,250.24,c,699.99", serialized);
        }

        [Test]
        public void SerializeValueTuple6Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m, false));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5,item6\n12,red,250.24,c,699.99,false", serialized);
        }

        [Test]
        public void SerializeValueTuple7Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m, false, 9000L));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5,item6,item7\n12,red,250.24,c,699.99,false,9000", serialized);
        }

        [Test]
        public void SerializeValueTuple8Test()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m, false, 9000L, new Version(1, 5, 134)));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5,item6,item7,item8\n12,red,250.24,c,699.99,false,9000,1.5.134", serialized);
        }

        [Test]
        public void SerializeValueTupleLongTest()
        {
            var tuple = CreateTuple((12, "red", 250.24f, 'c', 699.99m, false, 9000L, new Version(1, 5, 134), false, true, 1, 2, 2));
            var serialized = CsvConverter.Serialize(tuple, Options);

            Assert.AreEqual("item1,item2,item3,item4,item5,item6,item7,item8,item9,item10,item11,item12,item13\n12,red,250.24,c,699.99,false,9000,1.5.134,false,true,1,2,2", serialized);
        }

        record TupleItems<TTuple>(TTuple Items) where TTuple: ITuple;

        private static TupleItems<TTuple> CreateTuple<TTuple>(TTuple tuple) where TTuple: ITuple
        {
            return new TupleItems<TTuple>(tuple);
        }
    }
}
