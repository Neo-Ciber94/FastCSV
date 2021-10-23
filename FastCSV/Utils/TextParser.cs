using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Utils
{
    /// <summary>
    /// An enumerator over a text.
    /// </summary>
    public struct TextParser
    {
        private readonly ReadOnlyMemory<char> text;
        private int pos;

        public TextParser(string text) : this(text.AsMemory()) { }

        public TextParser(ReadOnlyMemory<char> text)
        {
            this.text = text;
            pos = -1;
        }

        public char Current => text.Span[pos];

        public bool CanConsume(ReadOnlySpan<char> other)
        {
            if (other.IsEmpty)
            {
                return false;
            }

            ReadOnlySpan<char> rest = Rest.Span;

            if (other.Length > rest.Length)
            {
                return false;
            }

            return rest.StartsWith(other);
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

        public ReadOnlyMemory<char> Rest
        {
            get
            {
                if (pos == -1)
                {
                    throw new InvalidOperationException("TextParser is not initialized, must call MoveNext() first");
                }

                if (pos >= text.Length)
                {
                    return ReadOnlyMemory<char>.Empty;
                }

                return text[pos..];
            }
        }

        public Optional<char> Peek
        {
            get
            {
                int length = text.Length;
                int next = pos + 1;

                if (next < length)
                {
                    return text.Span[next];
                }

                return default;
            }
        }

        public Optional<char> Next()
        {
            if (MoveNext())
            {
                return Current;
            }
            else
            {
                return default;
            }
        }

        public bool MoveNext()
        {
            int next = pos + 1;

            if (next <= text.Length)
            {
                pos = next;
            }

            return next < text.Length;
        }

        public bool HasNext()
        {
            int next = pos + 1;
            return next < text.Length;
        }

        public TextParser GetEnumerator() => this;
    }
}
