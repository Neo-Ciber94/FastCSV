using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Numerics;

namespace FastCSV.Internal
{
    internal static class TypeHelper
    {
        /// <summary>
        /// Attemps to determine the expected type from the given string value.
        /// </summary>
        /// <param name="value">Value to get the type.</param>
        /// <returns>The expected type for the given string or null if cannot be determined.</returns>
        public static Type? GetTypeFromString(ReadOnlySpan<char> value)
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

            // This should be before DateTime
            if (IsVersion(value))
            {
                return typeof(Version);
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
            int decimalPointIndex = value.IndexOf('.');
            int exponentIndex = value.IndexOf("E", StringComparison.OrdinalIgnoreCase);

            if (value[0] == '-' || value[0] == '+')
            {
                value = value[1..];
            }

            if (value.Trim().IsEmpty)
            {
                return false;
            }

            // Special cases
            if (value[0] == 'E' || value == ".E")
            {
                return false;
            }

            if (decimalPointIndex >= 0 || exponentIndex >= 0)
            {
                bool hasDecimalPoint = false;
                int length = value.Length;

                for (int i = 0; i < length; i++)
                {
                    char c = value[i];

                    if (c == '.')
                    {
                        if (hasDecimalPoint)
                        {
                            return false;
                        }

                        hasDecimalPoint = true;
                        continue;
                    }

                    if (c == 'E' || c == 'e')
                    {
                        if ((i + 1) < length)
                        {
                            char next = value[i + 1];

                            if (next == '+' || next == '-')
                            {
                                i += 2;
                            }
                            else
                            {
                                i += 1;
                            }

                            // The rest of the value must be a number
                            for (int j = i; j < length; j++)
                            {
                                if (!char.IsNumber(value[j]))
                                {
                                    return false;
                                }
                            }

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (!char.IsNumber(c))
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var c in value)
            {
                if (!char.IsNumber(c))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsDateTime(ReadOnlySpan<char> value)
        {
            return DateTime.TryParse(value, out _);
        }

        private static bool IsTimeSpan(ReadOnlySpan<char> value)
        {
            return TimeSpan.TryParse(value, out _);
        }

        private static bool IsDateTimeOffset(ReadOnlySpan<char> value)
        {
            return DateTimeOffset.TryParse(value, out _);
        }

        private static bool IsGuid(ReadOnlySpan<char> value)
        {
            return Guid.TryParse(value, out _);
        }

        private static bool IsVersion(ReadOnlySpan<char> value)
        {
            // We use 9.X.X.X as the max version to allow IPAddress to be parsed
            return Version.TryParse(value, out Version? v) && v.Major <= 9;
        }

        private static bool IsIPAddress(ReadOnlySpan<char> value)
        {
            return IPAddress.TryParse(value, out _);
        }

        private static bool IsIPEndpint(ReadOnlySpan<char> value)
        {
            return IPEndPoint.TryParse(value, out _);
        }

        private static Type? GetTypeFromNumber(ReadOnlySpan<char> digits)
        {
            Debug.Assert(IsNumeric(digits));

            int decimalPointIndex = digits.IndexOf('.');
            int exponentIndex = digits.IndexOf("E", StringComparison.OrdinalIgnoreCase);

            if (decimalPointIndex >= 0 || exponentIndex >= 0)
            {
                if (float.TryParse(digits, out float f) && float.IsFinite(f))
                {
                    return typeof(float);
                }

                if (double.TryParse(digits, out double d) && double.IsFinite(d))
                {
                    return typeof(double);
                }
            }
            else
            {
                if (int.TryParse(digits, out _))
                {
                    return typeof(int);
                }

                if (long.TryParse(digits, out _))
                {
                    return typeof(long);
                }

                if (uint.TryParse(digits, out _))
                {
                    return typeof(uint);
                }

                if (ulong.TryParse(digits, out _))
                {
                    return typeof(ulong);
                }

                return typeof(BigInteger);
            }

            return null;
        }
    }
}
