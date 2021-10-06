using System;
using System.Collections;
using System.Globalization;

namespace FastCSV.Converters.Collections
{
    internal class BitArrayConverter<TBit> : CsvCollectionConverter<BitArray, TBit> where TBit: IComparable, IComparable<TBit>, IEquatable<TBit>, IConvertible
    {
        public BitArrayConverter()
        {
            CheckTypeIsValid(typeof(TBit));
        }

        public override void AddItem(BitArray collection, int index, Type elementType, TBit item)
        {
            CheckTypeIsValid(elementType);

            collection[index] = item.ToBoolean(CultureInfo.InvariantCulture);
        }

        public override BitArray CreateCollection(Type elementType, int length)
        {
            CheckTypeIsValid(elementType);
            return new BitArray(length);
        }

        public override bool TrySerialize(BitArray value, ref CsvSerializeState state)
        {
            // SAFETY: Is a builtin converter
            ICsvValueConverter intConverter = state.GetConverter(typeof(int))!;
            int length = value.Length;

            for (int i = 0; i < length; i++)
            {
                int result = value[i] ? 1 : 0;
                if (!intConverter.TrySerialize(result, typeof(int), ref state))
                {
                    return false;
                }
            }

            return true;
        }

        private void CheckTypeIsValid(Type type)
        {
            switch (type)
            {
                case Type _ when type == typeof(bool):
                case Type _ when type == typeof(byte):
                case Type _ when type == typeof(short):
                case Type _ when type == typeof(int):
                case Type _ when type == typeof(long):
                case Type _ when type == typeof(sbyte):
                case Type _ when type == typeof(ushort):
                case Type _ when type == typeof(uint):
                case Type _ when type == typeof(ulong):
                    break;
                default:
                    throw new InvalidOperationException($"{GetType().GetGenericTypeDefinition()} only supports boolean and interger types");
            }
        }
    }
}
