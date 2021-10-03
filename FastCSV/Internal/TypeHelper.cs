using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Numerics;

namespace FastCSV.Internal
{
    internal static class TypeHelper
    {
        private const int IntDigitCount = 10;
        private const int LongDigitCount = 19;
        private const int FloatDigitCount = 39;
        private const int DoubleDigitCount = 309;

        /// <summary>
        /// Attemps to determine the expected type from the given <see cref="string"/> value.
        /// </summary>
        /// <param name="value">Value to get the type.</param>
        /// <returns>The expected type for the given string or null if cannot be determined.</returns>
        public static Type? GetTypeFromString(string value)
        {
            value = value.Trim();

            if (value.Length == 0)
            {
                return null;
            }

            if (value.Length == 1 && !char.IsNumber(value[0]))
            {
                return typeof(char);
            }

            if (IsBoolean(value))
            {
                return typeof(bool);
            }

            if (IsNumeric(value))
            {
                return GetTypeFromNumber(value);
            }

            if (IsDateTime(value))
            {
                return typeof(DateTime);
            }

            if (IsTimeSpan(value))
            {
                return typeof(TimeSpan);
            }

            if (IsDateTimeOffset(value))
            {
                return typeof(DateTimeOffset);
            }

            if (IsGuid(value))
            {
                return typeof(Guid);
            }

            if (IsVersion(value))
            {
                return typeof(Version);
            }

            if (IsIPAddress(value))
            {
                return typeof(IPAddress);
            }

            if (IsIPEndpint(value))
            {
                return typeof(IPEndPoint);
            }

            return null;
        }

        private static bool IsBoolean(ReadOnlySpan<char> value)
        {
            if (value.Length <= 5)
            {
                Span<char> lowerCaseValue = stackalloc char[value.Length];
                value.ToLowerInvariant(lowerCaseValue);

                if (lowerCaseValue.SequenceEqual("true") || lowerCaseValue.SequenceEqual("false"))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsNumeric(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty)
            {
                return false;
            }

            bool hasDecimalPoint = false;

            if (value[0] == '-' || value[0] == '+')
            {
                value = value[1..];
            }

            foreach (var c in value)
            {
                if (c == '.' && hasDecimalPoint == false)
                {
                    hasDecimalPoint = true;
                    continue;
                }

                if (!char.IsNumber(c))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsDateTime(string value)
        {
            return DateTime.TryParse(value, out _);
        }

        private static bool IsTimeSpan(string value)
        {
            return TimeSpan.TryParse(value, out _);
        }

        private static bool IsDateTimeOffset(string value)
        {
            return DateTimeOffset.TryParse(value, out _);
        }

        private static bool IsGuid(string value)
        {
            return Guid.TryParse(value, out _);
        }

        private static bool IsVersion(string value)
        {
            return Version.TryParse(value, out _);
        }

        private static bool IsIPAddress(string value)
        {
            return IPAddress.TryParse(value, out _);
        }

        private static bool IsIPEndpint(string value)
        {
            return IPEndPoint.TryParse(value, out _);
        }

        private static Type? GetTypeFromNumber(ReadOnlySpan<char> digits)
        {
            Debug.Assert(IsNumeric(digits));

            int decimalPointIndex = digits.IndexOf('.');
            bool hasSign = false;
            int sign = 1;

            if (digits[0] == '+' || digits[0] == '-')
            {
                hasSign = true;
                sign = digits[0] == '+' ? 1 : -1;
                digits = digits[1..];
            }

            if (decimalPointIndex >= 0)
            {
                if (digits.Length <= (FloatDigitCount + 1))
                {
                    return typeof(float);
                }

                if (digits.Length <= (DoubleDigitCount + 1))
                {
                    return typeof(double);
                }
            }
            else
            {
                int signCount = hasSign ? 1 : 0;

                if (digits.Length <= (IntDigitCount + signCount))
                {
                    return typeof(int);
                }

                if (digits.Length <= (LongDigitCount + signCount))
                {
                    return typeof(long);
                }

                return typeof(BigInteger);
            }

            return null;
        }
    }
}
