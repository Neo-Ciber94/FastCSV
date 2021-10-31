using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV.Utils
{
    public struct TextParser
    {
        private readonly ReadOnlyMemory<char> text;
        private Optional<char> next;
        private int pos;

        public TextParser(ReadOnlyMemory<char> text)
        {
            this.text = text;
            pos = 0;
            next = default;
        }

        public TextParser(string text) : this(text.AsMemory()) { }

        public TextParser this[Range range]
        {
            get
            {
                var (index, count) = range.GetOffsetAndLength(Rest.Length);
                return Slice(index, count);
            }
        }

        public ReadOnlyMemory<char> Rest
        {
            get => text[pos..];
        }

        public int Count => Rest.Length;

        public bool CanConsume(ReadOnlySpan<char> other)
        {
            if (other.IsEmpty)
            {
                return false;
            }

            var rest = Rest;

            if (other.Length > rest.Length)
            {
                return false;
            }

            return rest.Span.StartsWith(other);
        }

        public int Consume(ReadOnlySpan<char> other)
        {
            if (other.IsEmpty)
            {
                return 0;
            }

            ReadOnlySpan<char> rest = Rest.Span;

            if (other.Length > rest.Length)
            {
                return 0;
            }

            if (rest.StartsWith(other))
            {
                pos += other.Length;
                return other.Length;
            }

            return 0;
        }

        public Optional<char> Peek()
        {
            if (pos >= text.Length)
            {
                next = default;
            }
            else
            {
                next = text.Span[pos];
            }

            return next;
        }

        public bool HasNext()
        {
            return pos < text.Length;
        }

        public Optional<char> Next()
        {
            var nextValue = Peek();

            if (nextValue.HasValue)
            {
                pos += 1;
            }

            return nextValue;
        }

        public TextParser Slice(int start)
        {
            return new TextParser(Rest.Slice(start));
        }

        public TextParser Slice(int start, int count)
        {
            return new TextParser(Rest.Slice(start, count));
        }

        /// <summary>
        /// Returns a <see cref="TextParser"/> ignoring the next leading whitespaces.
        /// </summary>
        /// <returns>A parser ignoring the leading whitespaces.</returns>
        public TextParser TrimStart()
        {
            return new TextParser(Rest.TrimStart());
        }

        /// <summary>
        /// Returns a <see cref="TextParser"/> ignoring the next trailing whitespaces.
        /// </summary>
        /// <returns>A parser ignoring the trailing whitespaces.</returns>
        public TextParser TrimEnd()
        {
            return new TextParser(Rest.TrimStart());
        }

        /// <summary>
        /// Returns a <see cref="TextParser"/> ignoring the leading and trailing whitespaces.
        /// </summary>
        /// <returns></returns>
        public TextParser Trim()
        {
            return new TextParser(Rest.Trim());
        }

        public override string ToString()
        {
            return Rest.ToString();
        }
    }
}
