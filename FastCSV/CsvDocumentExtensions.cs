using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FastCSV
{
    /// <summary>
    /// Extension methods for <see cref="ICsvDocument"/>.
    /// </summary>
    public static class CsvDocumentExtensions
    {
        /// <summary>
        /// Creates a <see cref="CsvDocument{T}"/> from the elements of the given enumerable.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="format">The format to use.</param>
        /// <returns>A csv document from the items of the enumerable.</returns>
        public static CsvDocument<T> ToCsvDocument<T>(this IEnumerable<T> enumerable, CsvFormat? format = null)
        {
            return new CsvDocument<T>(enumerable, format);
        }

        /// <summary>
        /// Writes the contents of this <see cref="ICsvDocument"/> to a file.
        /// </summary>
        /// <param name="document">The source document.</param>
        /// <param name="path">The path of the file.</param>
        /// <param name="append">Whether if write the data at the end of the file.</param>
        public static void CopyToFile(this ICsvDocument document, string path, bool append = false)
        {
            CsvWriter.WriteToFile(document, document.Header, path, false, append);
        }

        /// <summary>
        /// Writes the contents of this <see cref="ICsvDocument"/> to a file asynchronously.
        /// </summary>
        /// <param name="document">The source document.</param>
        /// <param name="path">The path of the file.</param>
        /// <param name="append">Whether if write the data at the end of the file.</param>
        public static Task CopyToFileAsync(this ICsvDocument document, string path, bool append = false, CancellationToken cancellationToken = default)
        {
            return CsvWriter.WriteToFileAsync(document, document.Header, path, false, append, cancellationToken);
        }
    }
}
