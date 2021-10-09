﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
                string actualMessage = message ?? $"Expected '{expected}' but was '{value}'";
                throw new ArgumentException(actualMessage);
            }
        }

        [Conditional("DEBUG")]
        public static void NotEquals<T>(T expected, T value, string? message = null)
        {
            var comparer = EqualityComparer<T>.Default;
            if (!comparer.Equals(expected, value))
            {
                string actualMessage = message ?? $"Expected '{expected}' but was '{value}'";
                throw new ArgumentException(actualMessage);
            }
        }

        [Conditional("DEBUG")]
        public static void True(bool condition, string? message = null, [CallerArgumentExpression("condition")] string? conditionExpression = null)
        {
            if (!condition)
            {
                string actualMessage = message ?? $"Expected true but was: '{conditionExpression}'";
                throw new InvalidOperationException(actualMessage);
            }
        }

        [Conditional("DEBUG")]
        public static void False(bool condition, string? message = null, [CallerArgumentExpression("condition")] string? conditionExpression = null)
        {
            if (condition)
            {
                string actualMessage = message ?? $"Expected false but was: '{conditionExpression}'";
                throw new InvalidOperationException(actualMessage);
            }
        }
    }
}
