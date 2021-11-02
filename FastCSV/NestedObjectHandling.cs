using System;

namespace FastCSV
{
    /// <summary>
    /// Defines how to handle nested objects when serializing/deserializing.
    /// </summary>
    public sealed record NestedObjectHandling
    {
        private readonly int _maxDepth;

        /// <summary>
        /// Default nested object handling with a max depth of 8.
        /// </summary>
        public static NestedObjectHandling Default { get; } = new NestedObjectHandling { MaxDepth = 8 };

        /// <summary>
        /// Max depth allowed for nested objects.
        /// </summary>
        public int MaxDepth 
        {
            get => _maxDepth;
            init 
            { 
                if (value < 0)
                {
                    throw new ArgumentException("Cannot be negative", nameof(MaxDepth));
                }

                _maxDepth = value;
            }
        }

        /// <summary>
        /// How handle nested objects reference loops, by default throws an error.
        /// </summary>
        public ReferenceLoopHandling ReferenceLoopHandling { get; init; } = ReferenceLoopHandling.Error;
    }
}