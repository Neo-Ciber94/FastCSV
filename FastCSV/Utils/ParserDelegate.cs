using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastCSV.Utils
{
    /// <summary>
    /// Stores the results of a parsing operation.
    /// </summary>
    public readonly struct ParseResult
    {
        /// <summary>
        /// A failed parse result.
        /// </summary>
        public static ParseResult Failed => new ParseResult();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ParseResult(object? result)
        {
            Result = result;
            IsSuccess = true;
        }

        /// <summary>
        /// Gets a successful parsing result with the given value.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParseResult Ok(object? result)
        {
            return new ParseResult(result);
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public object? Result 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get; 
        }

        /// <summary>
        /// Gets or sets a value indicating whether the parse operation is successful.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the parse is successful; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get; 
        }
    }

    /// <summary>
    /// Delegate used by <see cref="CsvUtility.CreateInstance{T}(Dictionary{string, string}, ParserDelegate)"/>.
    /// </summary>
    /// <param name="key">The name of the field or property being parse.</param>
    /// <param name="value">The string value of the property to parse.</param>
    /// <returns>The result of the parsing operation.</returns>
    public delegate ParseResult ParserDelegate(string key, string value);
}
