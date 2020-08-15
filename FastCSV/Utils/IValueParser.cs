using System;
using System.Collections.Generic;
using System.Text;

namespace FastCSV.Utils
{
    /// <summary>
    /// Represents a parser of a csv field value.
    /// </summary>
    public interface IValueParser
    {
        /// <summary>
        /// Attempts to parse the specified value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="key">A key that represent the name of the field to parse.</param>
        /// <param name="result">The result of the parse.</param>
        /// <returns><c>true</c> if the value is parsed, otherwise <c>false</c>.</returns>
        bool TryParse(string value, string key, out object? result);
    }
}
