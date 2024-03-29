﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FastCSV.Internal
{
    internal static class Requires
    {
        [Conditional("DEBUG")]
        public static void Equals<T>(T expected, T value, string? message = null)
        {
            var comparer = EqualityComparer<T>.Default;
            if (!comparer.Equals(expected, value))
            {
                string actualMessage = message ?? $"Required '{expected}' but was '{value}'";
                throw new ArgumentException(actualMessage);
            }
        }

        [Conditional("DEBUG")]
        public static void NotEquals<T>(T expected, T value, string? message = null)
        {
            var comparer = EqualityComparer<T>.Default;
            if (!comparer.Equals(expected, value))
            {
                string actualMessage = message ?? $"Required '{expected}' but was '{value}'";
                throw new ArgumentException(actualMessage);
            }
        }

        [Conditional("DEBUG")]
        public static void True(bool condition, string? message = null)
        {
            if (!condition)
            {
                message ??= $"Required true";
                throw new InvalidOperationException(message);
            }
        }

        [Conditional("DEBUG")]
        public static void False(bool condition, string? message = null)
        {
            if (condition)
            {
                message ??= $"Required false";
                throw new InvalidOperationException(message);
            }
        }
    }
}
