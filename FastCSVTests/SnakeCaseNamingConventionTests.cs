using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FastCSV.Tests
{
    [TestFixture]
    public class SnakeCaseNamingConventionTests
    {
        [Test()]
        public void ConvertTest()
        {
            var snakeCase = CsvNamingConvention.SnakeCase;

            Assert.AreEqual("my_home", snakeCase.Convert("MyHome"));
            Assert.AreEqual("your_house", snakeCase.Convert("yourHouse"));
            Assert.AreEqual("__private__", snakeCase.Convert("__Private__"));
        }
    }
}
