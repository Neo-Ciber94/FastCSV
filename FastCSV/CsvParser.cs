using System;
using System.IO;
using System.Linq;
using System.Text;
using FastCSV.Collections;
using FastCSV.Utils;

namespace FastCSV
{
    public partial class CsvParser : IDisposable
    {
        private ArrayBuilder<string> _record;
        private StringBuilder _currentField;

        private readonly CsvFormat _format;
        private StreamReader? _reader;
        private TextParser _lineParser;

        private bool hasQuote = false;
        Position currentPosition = Position.Zero;
        Position quotePosition = Position.Zero;

        public CsvParser(StreamReader reader, CsvFormat? format = null)
        {
            _record = new ArrayBuilder<string>(32);
            _currentField = StringBuilderCache.Acquire(256);
            _reader = reader;
            _format = format ?? CsvFormat.Default;
        }

        public bool IsDone => _reader?.EndOfStream?? true;

        public Stream? BaseStream => _reader?.BaseStream;

        public string[]? ParseNext()
        {
            ThrowIfDisposed();

            if (_reader!.EndOfStream)
            {
                return null;
            }

            // If the record don't contains multi-line values, this outer loop will only run once
            while (true)
            {
                string? line = _reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                MoveToNextLine();

                // Ignore empty entries if the format don't allow whitespaces
                if (_format.IgnoreWhitespace && string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Parse the next line
                ParseLine(line);

                // Exit if we aren't in a quote
                if (!hasQuote)
                {
                    break;
                }
            }

            if (hasQuote)
            {
                ThrowException(CsvFormatError.UnclosedQuote);
            }

            string[]? result = _record.ToArray();
            _record.Clear();
            return result;
        }

        private void ParseLine(string line)
        {
            string delimiter = _format.Delimiter;
            string quote = _format.Quote;

            _lineParser = new TextParser(line);

            while (true)
            {
                Optional<char> next = _lineParser.Peek();

                if (!next.HasValue)
                {
                    break;
                }

                char nextChar = next.Value;
                MoveToNextChar();

                // We ignore any CR (carrier return) or LF (line-break)
                if (!hasQuote && (nextChar == '\r' || nextChar == '\n'))
                {
                    continue;
                }

                if (_lineParser.CanConsume(delimiter))
                {
                    ParseWithDelimiter(nextChar);
                }
                else if (_lineParser.CanConsume(quote))
                {
                    ParseWithQuote();
                }
                else
                {
                    _currentField.Append(nextChar);
                    _lineParser.Next();
                }
            }

            if (hasQuote)
            {
                if (!_format.IgnoreNewLine)
                {
                    _currentField.AppendLine();
                }
            }
            else
            {
                // Add the last record value
                WriteCurrentFieldToRecord();
            }
        }

        private void ParseWithDelimiter(char nextChar)
        {
            _lineParser.Consume(_format.Delimiter);

            if (hasQuote)
            {
                _currentField.Append(nextChar);
            }
            else
            {
                WriteCurrentFieldToRecord();
            }
        }

        private void ParseWithQuote()
        {
            string delimiter = _format.Delimiter;
            string quote = _format.Quote;
            QuoteStyle style = _format.Style;

            if (_currentField.Length > 0)
            {
                if (!hasQuote && _lineParser.HasNext() && !_lineParser[quote.Length..].TrimStart().CanConsume(delimiter))
                {
                    ThrowException(CsvFormatError.ExpectedEscapeQuote);
                }
            }

            if (hasQuote)
            {
                // If the next char is a quote, the current is an escape so ignore it and append the next char
                // Example: ""red"",other => "red",other
                if (_lineParser.Slice(quote.Length).CanConsume(quote) && _lineParser.Consume(quote) > 0)
                {
                    MoveToNextChar();

                    if (style != QuoteStyle.Never)
                    {
                        _currentField.Append(_lineParser.Peek().Value);
                    }
                }
                else
                {
                    switch (style)
                    {
                        case QuoteStyle.Always:
                            _currentField.Append(quote);
                            break;
                        case QuoteStyle.Never:
                            break;
                        case QuoteStyle.WhenNeeded:
                            if (!_lineParser.HasNext() || !_lineParser.TrimStart().CanConsume(delimiter))
                            {
                                _currentField.Append(quote);
                            }
                            break;
                    }

                    // Quote is close if there is no more elements or the next charater is a delimiter:
                    // "Field", "Field"
                    //       ^--end   ^--end                 
                    TextParser slice = _lineParser.Slice(quote.Length).TrimStart();

                    if (!slice.HasNext() || slice.CanConsume(delimiter))
                    {
                        hasQuote = false;
                    }
                }
            }
            else
            {
                switch (style)
                {
                    case QuoteStyle.Always:
                        _currentField.Append(quote);
                        break;
                    case QuoteStyle.Never:
                        break;
                    case QuoteStyle.WhenNeeded:
                        _currentField.Append(quote);
                        break;
                }

                quotePosition = currentPosition;
                hasQuote = true;
            }

            _lineParser.Consume(quote);
        }

        private void WriteCurrentFieldToRecord()
        {
            if (_format.IgnoreWhitespace)
            {
                _currentField.Trim();
            }

            string delimiter = _format.Delimiter;
            string quote = _format.Quote;
            QuoteStyle style = _format.Style;

            switch (style)
            {
                case QuoteStyle.Always:
                    if (!_currentField.StartsWith(quote) && !_currentField.EndsWith(quote))
                    {
                        _currentField.PadLeft(quote);
                        _currentField.PadRight(quote);
                    }
                    break;
                case QuoteStyle.Never:
                    if (_currentField.StartsWith(quote) && _currentField.EndsWith(quote))
                    {
                        _currentField.TrimStartOnce(quote);
                        _currentField.TrimEndOnce(quote);
                    }
                    break;
                case QuoteStyle.WhenNeeded:
                    break;
            }

            if (_currentField.Contains(quote) || _currentField.Contains(delimiter) || _currentField.Contains('\n'))
            {
                if (!_currentField.StartsWithIgnoreWhiteSpace(quote) || !_currentField.EndsWithIgnoreWhiteSpace(quote))
                {
                    ThrowException(CsvFormatError.ExpectedEncloseWithQuote);
                }
            }

            _record.Add(_currentField.ToString());
            _currentField.Clear();
        }

        private void MoveToNextLine()
        {
            currentPosition = currentPosition
                .AddLine(1)
                .WithOffset(0);
        }

        private void MoveToNextChar()
        {
            currentPosition = currentPosition.AddOffset(1);
        }

        private void ThrowIfDisposed()
        {
            if (_reader == null)
            {
                throw new ObjectDisposedException($"{GetType()} is already disposed");
            }
        }

        public void Dispose()
        {
            if (_reader == null)
            {
                return;
            }

            StringBuilderCache.Release(ref _currentField!);
            _record.Dispose();
            _reader!.Dispose();
            _reader = null;

            GC.SuppressFinalize(this);
        }

        ~CsvParser()
        {
            Dispose();
        }
    }
}
