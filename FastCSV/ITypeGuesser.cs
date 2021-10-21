using System;

namespace FastCSV
{
    /// <summary>
    /// Provides a method to attempt to determine the <see cref="Type"/> of a value from a string value.
    /// </summary>
    public interface ITypeGuesser
    {
        /// <summary>
        /// Attempts to guess the <see cref="Type"/> from a string value.
        /// </summary>
        /// <param name="s">The value to determine the type.</param>
        /// <returns>The type of the value or null if cannot be determine.</returns>
        Type? GetTypeFromString(ReadOnlySpan<char> s);
    }
}
