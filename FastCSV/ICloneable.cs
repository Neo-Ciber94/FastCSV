using System;

namespace FastCSV
{
    /// <summary>
    /// Provides a way for cloning objects.
    /// </summary>
    /// <typeparam name="T">Type of the instance to clone.</typeparam>
    /// <seealso cref="ICloneable" />
    public interface ICloneable<T> : ICloneable where T : ICloneable, ICloneable<T>
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A clone of this instance</returns>
        new T Clone();

        object ICloneable.Clone() => Clone();
    }
}
