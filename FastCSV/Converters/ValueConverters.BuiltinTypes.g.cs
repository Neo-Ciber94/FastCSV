////////////////// GENERATED CODE, DO NOT EDIT //////////////////

using System;
using System.Collections.Generic;

namespace FastCSV.Converters
{
	public static partial class ValueConverters
	{
		/// <summary>
		/// Builtin types converters.
		/// </summary>
		private static readonly IReadOnlyDictionary<Type, IValueConverter> BuiltInConverters = new Dictionary<Type, IValueConverter>()
		{
			{ typeof(System.Boolean),                  new BoolValueConverter() },
			{ typeof(System.Char),                     new CharValueConverter() },
			{ typeof(System.Byte),                     new ByteValueConverter() },
			{ typeof(System.Int16),                    new ShortValueConverter() },
			{ typeof(System.Int32),                    new IntValueConverter() },
			{ typeof(System.Int64),                    new LongValueConverter() },
			{ typeof(System.Single),                   new FloatValueConverter() },
			{ typeof(System.Double),                   new DoubleValueConverter() },
			{ typeof(System.SByte),                    new SByteValueConverter() },
			{ typeof(System.UInt16),                   new UShortValueConverter() },
			{ typeof(System.UInt32),                   new UIntValueConverter() },
			{ typeof(System.UInt64),                   new ULongValueConverter() },
			{ typeof(System.Decimal),                  new DecimalValueConverter() },
			{ typeof(System.String),                   new StringValueConverter() },
			{ typeof(System.Half),                     new HalfValueConverter() },
			{ typeof(System.DateTime),                 new DateTimeValueConverter() },
			{ typeof(System.DateTimeOffset),           new DateTimeOffsetValueConverter() },
			{ typeof(System.Numerics.BigInteger),      new BigIntegerValueConverter() },
			{ typeof(System.Guid),                     new GuidValueConverter() },
			{ typeof(System.Version),                  new VersionValueConverter() },
			{ typeof(System.TimeSpan),                 new TimeSpanValueConverter() },
			{ typeof(System.Net.IPAddress),            new IPAddressValueConverter() },
			{ typeof(System.Net.IPEndPoint),           new IPEndPointValueConverter() },
			{ typeof(System.IntPtr),                   new IntPtrValueConverter() },
			{ typeof(System.UIntPtr),                  new UIntPtrValueConverter() },
		};
	}
}
