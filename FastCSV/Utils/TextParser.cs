using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV.Utils
{
    public struct TextParser
    {
        private readonly ReadOnlyMemory<char> text;
        private Optional<char> nextValue;
        private int pos;

        public TextParser(ReadOnlyMemory<char> text)
        {
            this.text = text;
            pos = 0;
            nextValue = default;
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
                nextValue = default;
            }
            else
            {
                nextValue = text.Span[pos];
            }

            return nextValue;
        }

        public bool HasNext()
        {
            return pos < text.Length;
        }

        public Optional<char> Next()
        {
            var next = Peek();

            if (next.HasValue)
            {
                pos += 1;
            }

            return next;
        }

        public TextParser Slice(int start)
        {
            return new TextParser(Rest.Slice(start));
        }

        public TextParser Slice(int start, int count)
        {
            return new TextParser(Rest.Slice(start, count));
        }
    }
}
