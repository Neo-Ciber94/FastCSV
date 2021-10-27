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
            using var reader = new CsvReader(stream);

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

            using var reader = new CsvReader(StreamHelper.CreateStreamFromString(csv), hasHeader: false);

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

            using var reader = new CsvReader(StreamHelper.CreateStreamFromString(csv), CsvFormat.Default.WithIgnoreWhitespace(false));

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


            using var reader = new CsvReader(StreamHelper.CreateStreamFromString(csv));
            CollectionAssert.AreEqual(new string[] { "\"code\"", "\"name\"" }, reader.Header);
            CollectionAssert.AreEqual(new string[] { "0012", "\"Headphones\"" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "\"0032\"", "RGB Keyboard" }, reader.Read());
        }

        [Test]
        public void EncloseWithDoubleQuote_WriteTest()
        {
            using var stream = new MemoryStream();
            using var writer = new CsvWriter(stream, leaveOpen: true);

            writer.WriteAll(new string[] { "\"code\"", "\"name\"" });
            writer.WriteAll(new string[] { "\"0012\"", "\"Headphones\"" });
            writer.WriteAll(new string[] { "0032", "\"RGB Keyboard\"" });

            stream.Position = 0;
            using var reader = new StreamReader(stream);
            string csv = reader.ReadToEnd();
            string newLine = Environment.NewLine;

            var expected = new StringBuilder();
            expected.AppendLine("\"code\",\"name\"");
            expected.AppendLine("\"0012\",\"Headphones\"");
            expected.AppendLine("0032,\"RGB Keyboard\"");

            Assert.AreEqual(expected.ToString(), csv);
        }

        // Fields enclosed with comma, double quote, newline should be enclosed with double quote

        [Test]
        public void FieldWithComma_ReadTest()
        {
            string csv = @"""Id,Identifier"",""name,alias""
01,""Mouse RGB, 256 colors""
03,""Keyboard, Black""";

            using var reader = new CsvReader(StreamHelper.CreateStreamFromString(csv));

            CollectionAssert.AreEqual(new string[] { "\"Id,Identifier\"", "\"name,alias\"" }, reader.Header);
            CollectionAssert.AreEqual(new string[] { "01", "\"Mouse RGB, 256 colors\"" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "03", "\"Keyboard, Black\"" }, reader.Read());
            Assert.Null(reader.Read());
        }

        [Test]
        public void FieldWithComman_WriteTest()
        {
            using var stream = new MemoryStream();

            using var writer = new CsvWriter(stream, leaveOpen: true);

            writer.WriteAll(new string[] { "Id,Identifier", "name,alias" });
            writer.WriteAll(new string[] { "03", "Face Mask, White, Grey" });
            writer.WriteAll(new string[] { "04", "HeadPhone, Clean Sound" });

            stream.Position = 0;
            using var reader = new StreamReader(stream);
            string csv = reader.ReadToEnd();
            string newLine = Environment.NewLine;

            Assert.AreEqual(
                $"\"Id,Identifier\",\"name,alias\"{newLine}03,\"Face Mask, White, Grey\"{newLine}04,\"HeadPhone, Clean Sound\"{newLine}", 
                csv
            );
        }

        [Test]
        public void FieldWithDoubleQuote_ReadTest()
        {
            string csv = @"""String """"name"""""", ""Number """"age""""""
                            ""Marie """"The Red"""" Jhonson"", ""21 """"two digits""""""
                            """"""J""""jane"", ""19 """"two digits""""""";

            using var reader = new CsvReader(StreamHelper.CreateStreamFromString(csv));

            CollectionAssert.AreEqual(new string[] { "\"String \"name\"\"", "\"Number \"age\"\"" }, reader.Header);
            CollectionAssert.AreEqual(new string[] { "\"Marie \"The Red\" Jhonson\"", "\"21 \"two digits\"\"" }, reader.Read());
            CollectionAssert.AreEqual(new string[] { "\"\"J\"jane\"", "\"19 \"two digits\"\"" }, reader.Read());
            Assert.Null(reader.Read());
        }

        [Test]
        public void FieldWithDoubleQuote_WriteTest()
        {
            using var stream = new MemoryStream();
            using var writer = new CsvWriter(stream, leaveOpen: true);

            writer.WriteAll(new string[] { "String \"name\"", "Number \"age\"" });
            writer.WriteAll(new string[] { "Marie \"The Red\" Jhonson", "21 \"two digits\"" });
            writer.WriteAll(new string[] { "\"J\"jane", "19 \"two digits\"" });

            stream.Position = 0;
            using var reader = new StreamReader(stream);
            string csv = reader.ReadToEnd();

            var expected = new StringBuilder();
            expected.AppendLine("\"String \"\"name\"\"\",\"Number \"\"age\"\"\"");
            expected.AppendLine("\"Marie \"\"The Red\"\" Jhonson\",\"21 \"\"two digits\"\"\"");
            expected.AppendLine("\"\"\"J\"\"jane\",\"19 \"\"two digits\"\"\"");

            Assert.AreEqual(expected.ToString(), csv);
        }

        [Test]
        public void FieldWithNewLine_ReadTest()
        {
            string csv = @"author,phrase
Nelson Mandela,""The greatest glory in living lies not in never falling, 
but in rising every time we fall""
Jhon Lennon, ""Life is what happens 
when you're busy making other plans""";

            using var reader = new CsvReader(StreamHelper.CreateStreamFromString(csv));
            CollectionAssert.AreEqual(new string[] { "author", "phrase" }, reader.Header);

            string newLine = Environment.NewLine;
            CollectionAssert.AreEqual(
                new string[] { 
                    @"Nelson Mandela", $"\"The greatest glory in living lies not in never falling, {newLine}but in rising every time we fall\"" 
                }, reader.Read());

            CollectionAssert.AreEqual(new string[] { 
                @"Jhon Lennon", $"\"Life is what happens {newLine}when you're busy making other plans\"" 
            }, reader.Read());

            Assert.Null(reader.Read());
        }

        [Test]
        public void FieldWithNewLine_WriteTest()
        {
            using var stream = new MemoryStream();
            using var writer = new CsvWriter(stream, leaveOpen: true);

            string newLine = Environment.NewLine;
            writer.Write("author", "phrase");
            writer.Write("Benjamin Franklin", $"Tell me and I forget.{newLine}Teach me and I remember.{newLine}Involve me and I learn");
            writer.Write("Abraham Lincoln", $"In the end, it's not the years in your life that count.{newLine}It's the life in your years.");

            stream.Position = 0;
            using var reader = new StreamReader(stream);
            string csv = reader.ReadToEnd();

            var expected = new StringBuilder();
            expected.AppendLine("author,phrase");
            expected.AppendLine($"Benjamin Franklin,\"Tell me and I forget.{newLine}Teach me and I remember.{newLine}Involve me and I learn\"");
            expected.AppendLine($"Abraham Lincoln,\"In the end, it's not the years in your life that count.{newLine}It's the life in your years.\"");
            Assert.AreEqual(expected.ToString(), csv);
        }

        // Escape double quote in field
    }
}
