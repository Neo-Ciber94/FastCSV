﻿using System;
using System.Threading.Tasks;

namespace FastCSV
{
    /// <summary>
    /// Extension methods for <see cref="ICsvDocument"/>.
    /// </summary>
    public static class CsvDocumentExtensions
    {
        /// <summary>
        /// Writes the contents of this <see cref="ICsvDocument"/> to a file.
        /// </summary>
        /// <param name="document">The source document.</param>
        /// <param name="path">The path of the file.</param>
        /// <param name="append">Whether if write the data at the end of the file.</param>
        [Obsolete("Use CsvDocument.CopyTo instead")]
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
        [Obsolete("Use CsvDocument.CopyToAsync instead")]
        public static Task CopyToFileAsync(this ICsvDocument document, string path, bool append = false)
        {
            CsvWriter.WriteToFile(document, document.Header, path, false, append);
            return Task.CompletedTask;
        }
    }
}
