using FastCSV.Converters;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV
{
    [TestFixture]
    public class CsvConverterPlainTypeTests
    {
        [Test]
        public void SerializeAndDeserializeCustomType()
        {
            // TODO: Add your test code here
            var answer = 42;
            Assert.That(answer, Is.EqualTo(42), "Some useful error message");
        }

        record OddOrEvenNumber(int Value) : IValueConverter<OddOrEvenNumber>
        {
            public bool IsEven => Value % 2 == 0;

            public string ConvertFrom(OddOrEvenNumber value)
            {
                throw new NotImplementedException();
            }

            public bool ConvertTo(ReadOnlySpan<char> s, out OddOrEvenNumber value)
            {
                throw new NotImplementedException();
            }
        }
    }
}