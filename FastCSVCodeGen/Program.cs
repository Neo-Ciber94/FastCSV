using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;

namespace FastCSVCodeGen
{
    class Program
    {
        private static readonly IReadOnlyDictionary<Type, string> Types = new Dictionary<Type, string>
        {
            { typeof(bool), "Bool" },
            { typeof(char), "Char" },
            { typeof(byte), "Byte" },
            { typeof(short), "Short" },
            { typeof(int), "Int" },
            { typeof(long), "Long" },
            { typeof(float), "Float" },
            { typeof(double), "Double" },
            { typeof(sbyte), "SByte" },
            { typeof(ushort), "UShort" },
            { typeof(uint), "UInt" },
            { typeof(ulong), "ULong" },
            { typeof(decimal), "Decimal" },
            { typeof(Half), nameof(Half) },
            { typeof(DateTime), nameof(DateTime) },
            { typeof(DateTimeOffset), nameof(DateTimeOffset) },
            { typeof(BigInteger), nameof(BigInteger) },
            { typeof(Guid), nameof(Guid) },
            { typeof(Version), nameof(Version) },
            { typeof(TimeSpan), nameof(TimeSpan) },
            { typeof(IPAddress), nameof(IPAddress) },
            { typeof(IPEndPoint), nameof(IPEndPoint) },
            { typeof(IntPtr), nameof(IntPtr) },
            { typeof(UIntPtr), nameof(UIntPtr) },
        };


        static void Main()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string path = @$"{basePath}..\..\..\..\FastCSV\\";

            string convertersPath = @$"{path}\\Converters\\";

            ValueConverterCodeGenerator.WriteTo(convertersPath, Types);
            BuiltInTypeCodeGenerator.WriteValueConverters(convertersPath, Types);

            Console.ReadKey();
        }
    }
}
