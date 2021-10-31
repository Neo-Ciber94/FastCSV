using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FastCSV.Extensions
{
    internal struct LineEnumerator : IEnumerator<string>, IEnumerable<string>
    {
        private static readonly string[] NewLineSeparator = new string[] { "\r\n", "\n", "\r" };

        private object _reader;
        private string? _currentLine;
        private int _index;

        public LineEnumerator(TextReader reader)
        {
            _reader = reader;
            _currentLine = null;
            _index = 0;
        }

        public LineEnumerator(string s)
        {
            _reader = s;
            _currentLine = null;
            _index = 0;
        }

        public string Current => _currentLine ?? string.Empty;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            if (_reader is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public LineEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            _currentLine = GetNextLine();
            return _currentLine != null;
        }

        private string? GetNextLine()
        {
            if (_reader is TextReader reader)
            {
                return reader.ReadLine();
            }

            if (_reader is string s)
            {
                for (int i = 0; i < NewLineSeparator.Length; i++)
                {
                    int splitIndex = s.IndexOf(NewLineSeparator[i], _index);

                    if (splitIndex >= 0)
                    {
                        string newLine = s.Substring(0, splitIndex);
                        _reader = s[(splitIndex + 1)..];
                        _index = splitIndex + 1;
                        return newLine;
                    }
                }

                if (_index >= s.Length)
                {
                    return null;
                }

                _index = s.Length;
                return s;
            }

            return null;
        }

        public void Reset()
        {
            if (_reader is string)
            {
                _index = 0;
                _currentLine = null;
                return;
            }

            throw new NotSupportedException();
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal static class LineEnumeratorHelper
    {
        public static LineEnumerator GetLines(this string self)
        {
            return new LineEnumerator(self);
        }

        public static LineEnumerator GetLines(this TextReader self)
        {
            return new LineEnumerator(self);
        }

        public static string? ReadLineAt(this string self, int index)
        {
            var lines = self.GetLines();
            return ReadLineAtInternal(lines, index);
        }

        public static string? ReadLineAt(this TextReader self, int index)
        {
            var lines = self.GetLines();
            return ReadLineAtInternal(lines, index);
        }

        private static string? ReadLineAtInternal(LineEnumerator lines, int index)
        {
            if (index < 0)
            {
                return null;
            }

            while (lines.MoveNext())
            {
                string line = lines.Current;

                if (index == 0)
                {
                    return line;
                }

                index -= 1;
            }

            return null;
        }
    }
}
