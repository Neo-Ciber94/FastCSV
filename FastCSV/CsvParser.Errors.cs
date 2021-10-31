using System.IO;
using FastCSV.Extensions;
using FastCSV.Utils;

namespace FastCSV
{
    enum CsvFormatError
    {
        UnclosedQuote,
        ExpectedEscapeQuote,
        ExpectedEncloseWithQuote
    }

    public partial class CsvParser
    {
        private const string UnclosedQuoteMsg = "A quote wasn't closed";
        private const string ExpectedEscapeQuoteMsg = "Expected a quote escape";
        private const string ExpectedEncloseWithQuote = "Fields containing a delimiter, newline or quote should be enclosed with double quote";

        private void ThrowException(CsvFormatError error)
        {
            throw error switch
            {
                CsvFormatError.UnclosedQuote => GetCsvFormatException(UnclosedQuoteMsg, quotePosition),
                CsvFormatError.ExpectedEscapeQuote => GetCsvFormatException(ExpectedEscapeQuoteMsg, quotePosition),
                CsvFormatError.ExpectedEncloseWithQuote => GetCsvFormatException(ExpectedEncloseWithQuote, quotePosition),
                _ => new CsvFormatException("Invalid csv format"),
            };
        }
        private CsvFormatException GetCsvFormatException(string message, Position position)
        {
            string? highLightText = HightLightText(BaseStream!, position.Line, position.Offset);

            if (highLightText == null)
            {
                return new CsvFormatException($"{message}: {position}");
            }
            else
            {
                return new CsvFormatException($"{message}: \n{highLightText}");
            }

            static string? HightLightText(Stream stream, int lineNumber, int offset)
            {
                var newStream = stream.Clone();

                if (newStream == null || lineNumber < 0)
                {
                    return null;
                }

                using var reader = new StreamReader(newStream);
                string? line = reader.ReadLineAt(lineNumber - 1);

                if (line == null)
                {
                    return null;
                }

                string point = "^";

                if (offset > 0)
                {
                    point = new string('-', offset - 1) + point;
                }

                return line + "\n" + point;
            }
        }
    }
}
