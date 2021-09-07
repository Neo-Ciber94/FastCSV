using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Utils;
using NUnit.Framework;

namespace FastCSV.Utils.Tests
{
    [TestFixture()]
    public class CsvUtilityTests
    {
        [Test]
        public void ToCsvStringTest()
        {
            var values = new string[] { "red", "blue", "green" };
            var csv = CsvUtility.ToCsvString(values, CsvFormat.Default);

            Assert.AreEqual("red,blue,green", csv);
        }

        [Test]
        public void ToCsvStringWithQuoteTest()
        {
            var values = new string[] { "Carl John", "Maria \"Red\" Gold", "Elena" };
            var csv = CsvUtility.ToCsvString(values, CsvFormat.Default);

            Assert.AreEqual("Carl John,\"Maria \"\"Red\"\" Gold\",Elena", csv);
        }

        [Test]
        public void ToCsvStringWithDelimiterTest()
        {
            var values = new string[] { "Mars", "Earth,Tierra", "Pluto" };
            var csv = CsvUtility.ToCsvString(values, CsvFormat.Default);

            Assert.AreEqual("Mars,\"Earth,Tierra\",Pluto", csv);
        }
    }
}
