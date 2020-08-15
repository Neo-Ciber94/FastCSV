using System.Collections.Generic;

namespace FastCSV
{
    /// <summary>
    /// Represents an in-memory csv document.
    /// </summary>
    /// <seealso cref="IEnumerable{CsvRecord}" />
    public interface ICsvDocument : IEnumerable<CsvRecord>
    {
        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public CsvHeader Header { get; }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public CsvFormat Format { get; }

        /// <summary>
        /// Gets the number of records in the document.
        /// </summary>
        /// <value>
        /// The number of records in the document.
        /// </value>
        public int Count { get; }

        /// <summary>
        /// Gets a value indicating whether this document is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this document is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty { get; }

        /// <summary>
        /// Gets the <see cref="CsvRecord"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="CsvRecord"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public CsvRecord this[int index] { get; }

        /// <summary>
        /// Writes the contents of this document to file.
        /// </summary>
        /// <param name="path">The path.</param>
        public void WriteContentsToFile(string path);
    }
}
