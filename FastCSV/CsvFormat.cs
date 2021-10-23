using System;
using System.Collections.Generic;

namespace FastCSV
{
    /// <summary>
    /// Represents the format used in a csv document.
    /// </summary>
    /// <seealso cref="FastCSV.ICloneable{FastCSV.CsvFormat}" />
    /// <seealso cref="System.IEquatable{FastCSV.CsvFormat}" />
    [Serializable]
    public class CsvFormat : ICloneable<CsvFormat>, IEquatable<CsvFormat?>
    {
        public const string DefaultDelimiter = ",";

        public const string DefautlQuote = "\"";

        public const QuoteStyle DefaultStyle = QuoteStyle.WhenNeeded;

        /// <summary>
        /// Gets the default format.
        /// </summary>
        /// <value>
        /// The default.
        /// </value>
        public static CsvFormat Default { get; } = new CsvFormat();

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormat"/> class.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="quote">The quote.</param>
        /// <param name="style">The style.</param>
        /// <param name="ignoreWhitespaces">if set to <c>true</c> leading and trailing whitespaces will be ignored.</param>
        /// <exception cref="ArgumentException">If the delimiter is equals to the quote</exception>
        public CsvFormat(string delimiter = DefaultDelimiter, string quote = DefautlQuote, QuoteStyle style = QuoteStyle.WhenNeeded, bool ignoreWhitespaces = true)
        {
            if (delimiter == quote)
            {
                throw new ArgumentException("Deliminter cannot be equals to the quote");
            }

            if (delimiter.Length == 0)
            {
                throw new ArgumentException("Delimiter cannot be empty");
            }

            if (quote.Length == 0)
            {
                throw new ArgumentException("Quote cannot be a empty");
            }

            Delimiter = delimiter;
            Quote = quote;
            Style = style;
            IgnoreWhitespace = ignoreWhitespaces;
        }

        /// <summary>
        /// Gets the delimiter.
        /// </summary>
        /// <value>
        /// The delimiter.
        /// </value>
        public string Delimiter { get; }

        /// <summary>
        /// Gets the quote.
        /// </summary>
        /// <value>
        /// The quote.
        /// </value>
        public string Quote { get; }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public QuoteStyle Style { get; }

        /// <summary>
        /// Gets a value indicating whether leading or trailing whitespaces in a field should be ignore.
        /// </summary>
        /// <value>
        ///   <c>true</c> if leading or trailing whitespaces should be ignore; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreWhitespace { get; }

        /// <summary>
        /// Gets a copy of this format with the specified delimiter.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>A copy of this format with the delimiter</returns>
        public CsvFormat WithDelimiter(string delimiter)
        {
            return new CsvFormat(delimiter, this.Quote, this.Style, this.IgnoreWhitespace);
        }

        /// <summary>
        /// Gets a copy of this format with the specified quote.
        /// </summary>
        /// <param name="quote">The quote.</param>
        /// <returns>A copy of this format with the quote</returns>
        public CsvFormat WithQuote(string quote)
        {
            return new CsvFormat(this.Delimiter, quote, this.Style, this.IgnoreWhitespace);
        }

        /// <summary>
        /// Gets a copy of this format with the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>A copy of this format with the style</returns>
        public CsvFormat WithStyle(QuoteStyle style)
        {
            return new CsvFormat(this.Delimiter, this.Quote, style, this.IgnoreWhitespace);
        }

        /// <summary>
        /// Gets a copy of this format with the specified ignoreWhitespaces.
        /// </summary>
        /// <param name="ignoreWhitespaces">The ignoreWhitespaces.</param>
        /// <returns>A copy of this format with the ignoreWhitespaces</returns>
        public CsvFormat WithIgnoreWhitespace(bool ignoreWhitespaces)
        {
            return new CsvFormat(this.Delimiter, this.Quote, this.Style, ignoreWhitespaces);
        }

        public override string ToString()
        {
            return $"{{{nameof(Delimiter)}={Delimiter}, {nameof(Quote)}={Quote}, {nameof(Style)}={Style}, {nameof(IgnoreWhitespace)}={IgnoreWhitespace}}}";
        }

        public CsvFormat Clone()
        {
            return new CsvFormat(Delimiter, Quote, Style, IgnoreWhitespace);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CsvFormat);
        }

        public bool Equals(CsvFormat? other)
        {
            return other != null &&
                   Delimiter == other.Delimiter &&
                   Quote == other.Quote &&
                   Style == other.Style &&
                   IgnoreWhitespace == other.IgnoreWhitespace;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Delimiter, Quote, Style, IgnoreWhitespace);
        }

        public static bool operator ==(CsvFormat? left, CsvFormat? right)
        {
            return EqualityComparer<CsvFormat?>.Default.Equals(left, right);
        }

        public static bool operator !=(CsvFormat? left, CsvFormat? right)
        {
            return !(left == right);
        }
    }
}
