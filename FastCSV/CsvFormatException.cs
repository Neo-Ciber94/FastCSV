using System;
using System.Runtime.Serialization;

namespace FastCSV
{

    /// <summary>
    /// An exception throw when fail to read or write a csv file.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class CsvFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormatException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CsvFormatException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvFormatException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public CsvFormatException(string message, Exception inner) : base(message, inner) { }

        protected CsvFormatException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
