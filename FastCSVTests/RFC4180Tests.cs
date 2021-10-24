using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Utils;
using NUnit.Framework;

namespace FastCSV
{
    /**
     * Tests for: https://datatracker.ietf.org/doc/html/rfc4180
     */
    [TestFixture]
    public class RFC4180Tests
    {
        [Test]
        public void RecordsSeparatedByNewLine_ReadTests()
        {
            string csv = @"name,price
                           Mouse Pad,299
                           Table,2450";

            var stream = StreamHelper.CreateStreamFromString(csv);
            using var reader = CsvReader.FromStream(stream);

            CollectionAssert.AreEqual(new string[] { "name", "price" }, reader.Header);
            CollectionAssert.AreEqual(new string[] { "Mouse Pad", "299" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "Table", "2450" }, reader.Read());
            Assert.Null(reader.Read());
        }

        [Test]
        public void RecordsSeparatedByNewLine_WriteTests()
        {
            using var stream = new MemoryStream();
            using var writer = new CsvWriter(stream, leaveOpen: true);
            writer.Write("name", "price");
            writer.Write("Keyboard", "129.99");
            writer.Write("Monitor", "5000");

            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var newLine = Environment.NewLine;
            Assert.AreEqual($@"name,price{newLine}Keyboard,129.99{newLine}Monitor,5000{newLine}", reader.ReadToEnd());
        }

        [Test]
        public void RecordsNoHeaderTest()
        {
            string csv = @"00,Small LED Lights
                           01,Fan
                           02,RGB Mouse";

            using var reader = CsvReader.FromStream(StreamHelper.CreateStreamFromString(csv), hasHeader: false);

            Assert.Null(reader.Header);
            CollectionAssert.AreEqual(new string[] { "00", "Small LED Lights" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "01", "Fan" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "02", "RGB Mouse" }, reader.Read());
            Assert.Null(reader.Read());
        }

        [Test]
        public void NoIgnoreWhiteSpace_ReadTest()
        {
            string newLine = Environment.NewLine;
            string csv = $" id, price  {newLine}01 , $249.00  {newLine}02,$300{newLine} 03 , $599.00";

            using var reader = CsvReader.FromStream(StreamHelper.CreateStreamFromString(csv), CsvFormat.Default.WithIgnoreWhitespace(false));

            CollectionAssert.AreEqual(new string[] { " id", " price  " }, reader.Header);
            CollectionAssert.AreEqual(new string[] { "01 ", " $249.00  " }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "02", "$300" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { " 03 ", " $599.00" }, reader.Read());
        }

        [Test]
        public void NoIgnoreWhiteSpace_WriteTest()
        {
            using var stream = new MemoryStream();
            using var writer = new CsvWriter(stream, CsvFormat.Default.WithIgnoreWhitespace(false), leaveOpen: true);

            writer.Write(" id", " numberName");
            writer.Write(" 1  ", " one ");
            writer.Write("21 ", " Twenty one");
            writer.Write(" 100", "One Hundred ");
            stream.Position = 0;

            using var reader = new StreamReader(stream);
            string csv = reader.ReadToEnd();

            string newLine = Environment.NewLine;
            Assert.AreEqual($" id, numberName{newLine} 1  , one {newLine}21 , Twenty one{newLine} 100,One Hundred {newLine}", csv);
        }

        [Test]
        public void EncloseWithDoubleQuote_ReadTest()
        {
            string csv = @"""code"",""name""
                          0012,""Headphones""
                        ""0032"",RGB Keyboard";


            using var reader = CsvReader.FromStream(StreamHelper.CreateStreamFromString(csv));
            CollectionAssert.AreEqual(new string[] { "\"code\"", "\"name\"" }, reader.Header);
            CollectionAssert.AreEqual(new string[] { "0012", "\"Headphones\"" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "\"0032\"", "RGB Keyboard" }, reader.Read());
        }

        [Test]
        public void EncloseWithDoubleQuote_WriteTest()
        {
            using var stream = new MemoryStream();
            using var writer = new CsvWriter(stream, leaveOpen: true);

            writer.WriteAll(new string[] { "\"code\"", "\"\"name\"" });
            writer.WriteAll(new string[] { "\"0012\"", "\"\"Headphones\"" });
            writer.WriteAll(new string[] { "0032", "\"\"RGB Keyboard\"" });
        }

        // Fields enclosed with comma, double quote, newline should be enclosed with double quote

        [Test]
        public void FieldWithComma_ReadTest()
        {
            string csv = @"""Id,Identifier"",""name,alias""
                             01,""Mouse RGB, 256 colors""
                             03,""Keyboard, Black""";

            using var reader = CsvReader.FromStream(StreamHelper.CreateStreamFromString(csv));

            CollectionAssert.AreEqual(new string[] { "Id,Identifier", "name,alias" }, reader.Header);
            CollectionAssert.AreEqual(new string[] { "01", "Mouse RGB, 256 colors" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "02", "Keyboard, Black" }, reader.Read());
            Assert.Null(reader.Read());
        }

        [Test]
        public void FieldWithCommand_WriteTest()
        {
            using var stream = new MemoryStream();

            using var writer = new CsvWriter(stream);
        }
    }
}
