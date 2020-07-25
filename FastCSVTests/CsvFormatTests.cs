using NUnit.Framework;
using System;

namespace FastCSV.Tests
{
    [TestFixture()]
    public class CsvFormatTests
    {
        [Test()]
        public void CsvFormatTest()
        {
            var format = new CsvFormat('\t', '\'', QuoteStyle.Always);

            Assert.AreEqual('\t', format.Delimiter);
            Assert.AreEqual('\'', format.Quote);
            Assert.AreEqual(QuoteStyle.Always, format.Style);

            Assert.Throws<ArgumentException>(() =>
            {
                var _ = new CsvFormat('\"', '\"');
            });
        }

        [Test()]
        public void WithDelimiterTest()
        {
            var format = new CsvFormat(',', '\"', QuoteStyle.WhenNeeded);
            var format2 = format.WithDelimiter('\t');

            Assert.AreEqual('\t', format2.Delimiter);
        }

        [Test()]
        public void WithQuoteTest()
        {
            var format = new CsvFormat(',', '\"', QuoteStyle.WhenNeeded);
            var format2 = format.WithQuote('\"');

            Assert.AreEqual('\"', format2.Quote);
        }

        [Test()]
        public void WithStyleTest()
        {
            var format = new CsvFormat(',', '\"', QuoteStyle.WhenNeeded);
            var format2 = format.WithStyle(QuoteStyle.Always);

            Assert.AreEqual(QuoteStyle.Always, format2.Style);
        }

        [Test()]
        public void WithIgnoreWithspaceTest()
        {
            var format = new CsvFormat(',', '\"', QuoteStyle.WhenNeeded);
            var format2 = format.WithIgnoreWhitespace(false);

            Assert.AreNotEqual(format2.IgnoreWhitespace, format.IgnoreWhitespace);
        }

        [Test()]
        public void CloneTest()
        {
            var format = new CsvFormat(',', '\"', QuoteStyle.WhenNeeded);
            var clone = format.Clone();
            Assert.IsTrue(format == clone);
        }

        [Test()]
        public void EqualsTest()
        {
            var format = new CsvFormat(',', '\"', QuoteStyle.WhenNeeded);
            Assert.IsTrue(format == CsvFormat.Default);
        }
    }
}