////////////////// GENERATED CODE, DO NOT EDIT //////////////////

using System;

namespace FastCSV
{
	public static partial class CsvConverter
	{
		/// <summary>
		/// Checks if the type can be serialize/deserialize with a builtin converter.
		/// </summary>
		/// <param name="type">The type<see cref="Type"/>Type to check.</param>
		/// <returns>The <c>true</c> if can be serialize/deserialize with a default converter.</returns>
		internal static bool IsBuiltInType(Type type)
		{
			return type.IsPrimitive
				|| type.IsEnum
				|| type == typeof(System.Decimal)
				|| type == typeof(System.Half)
				|| type == typeof(System.DateTime)
				|| type == typeof(System.DateTimeOffset)
				|| type == typeof(System.Numerics.BigInteger)
				|| type == typeof(System.Guid)
				|| type == typeof(System.Version)
				|| type == typeof(System.TimeSpan)
				|| type == typeof(System.Net.IPAddress)
				|| type == typeof(System.Net.IPEndPoint);
		}
	}
}
