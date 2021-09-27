////////////////// GENERATED CODE, DO NOT EDIT //////////////////

using FastCSV.Converters.Builtin;

namespace FastCSV.Converters
{
	internal partial class CsvDefaultValueConverterProvider
	{
		private partial void Initialize()
		{
			_converters.Add(typeof(System.Boolean)                  , new BoolValueConverter());
			_converters.Add(typeof(System.Char)                     , new CharValueConverter());
			_converters.Add(typeof(System.Byte)                     , new ByteValueConverter());
			_converters.Add(typeof(System.Int16)                    , new ShortValueConverter());
			_converters.Add(typeof(System.Int32)                    , new IntValueConverter());
			_converters.Add(typeof(System.Int64)                    , new LongValueConverter());
			_converters.Add(typeof(System.Single)                   , new FloatValueConverter());
			_converters.Add(typeof(System.Double)                   , new DoubleValueConverter());
			_converters.Add(typeof(System.SByte)                    , new SByteValueConverter());
			_converters.Add(typeof(System.UInt16)                   , new UShortValueConverter());
			_converters.Add(typeof(System.UInt32)                   , new UIntValueConverter());
			_converters.Add(typeof(System.UInt64)                   , new ULongValueConverter());
			_converters.Add(typeof(System.Decimal)                  , new DecimalValueConverter());
			_converters.Add(typeof(System.String)                   , new StringValueConverter());
			_converters.Add(typeof(System.Half)                     , new HalfValueConverter());
			_converters.Add(typeof(System.DateTime)                 , new DateTimeValueConverter());
			_converters.Add(typeof(System.DateTimeOffset)           , new DateTimeOffsetValueConverter());
			_converters.Add(typeof(System.Numerics.BigInteger)      , new BigIntegerValueConverter());
			_converters.Add(typeof(System.Guid)                     , new GuidValueConverter());
			_converters.Add(typeof(System.Version)                  , new VersionValueConverter());
			_converters.Add(typeof(System.TimeSpan)                 , new TimeSpanValueConverter());
			_converters.Add(typeof(System.Net.IPAddress)            , new IPAddressValueConverter());
			_converters.Add(typeof(System.Net.IPEndPoint)           , new IPEndPointValueConverter());
			_converters.Add(typeof(System.IntPtr)                   , new IntPtrValueConverter());
			_converters.Add(typeof(System.UIntPtr)                  , new UIntPtrValueConverter());
		}
	}
}
