using System;
using System.Collections.Generic;

namespace FastCSV
{
    /// <summary>
    /// Provides an skeleton for represent a csv record.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerable{System.String}" />
    public interface ICsvRecord : IEnumerable<string>
    {
        /// <summary>
        /// Gets the number of fields in the record.
        /// </summary>
        /// <value>
        /// The number of fields in the record.
        /// </value>
        public int Length { get; }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public CsvFormat Format { get; }

        /// <summary>
        /// Gets the header this record is associated.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public CsvHeader? Header { get; }

        /// <summary>
        /// Gets the <see cref="System.String"/> value at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string this[int index] { get; }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key] { get; }

        /// <summary>
        /// Gets the <see cref="Span{System.String}"/> with the specified range.
        /// </summary>
        /// <value>
        /// The <see cref="Span{System.String}"/>.
        /// </value>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public Span<string> this[Range range] { get; }
    }
}
