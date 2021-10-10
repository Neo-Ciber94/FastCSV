using System;
using System.Runtime.CompilerServices;

namespace FastCSV.Utils
{
    internal enum TupleKind
    {
        ValueType, Reference
    }

    internal static class TupleUtils
    {
        public static Type GetValueTupleTGenericDefinition(int length)
        {
            return length switch
            {
                1 => typeof(ValueTuple<>),
                2 => typeof(ValueTuple<,>),
                3 => typeof(ValueTuple<,,>),
                4 => typeof(ValueTuple<,,,>),
                5 => typeof(ValueTuple<,,,,>),
                6 => typeof(ValueTuple<,,,,,>),
                7 => typeof(ValueTuple<,,,,,,>),
                8 => typeof(ValueTuple<,,,,,,,>),
                _ => throw new Exception($"Invalid value type length: {length}")
            };
        }

        public static Type GetReferenceTupleTGenericDefinition(int length)
        {
            return length switch
            {
                1 => typeof(Tuple<>),
                2 => typeof(Tuple<,>),
                3 => typeof(Tuple<,,>),
                4 => typeof(Tuple<,,,>),
                5 => typeof(Tuple<,,,,>),
                6 => typeof(Tuple<,,,,,>),
                7 => typeof(Tuple<,,,,,,>),
                8 => typeof(Tuple<,,,,,,,>),
                _ => throw new Exception($"Invalid value type length: {length}")
            };
        }

        public static Type GetTupleGenericDefinition(int length, TupleKind kind)
        {
            return kind switch
            {
                TupleKind.ValueType => GetValueTupleTGenericDefinition(length),
                TupleKind.Reference => GetReferenceTupleTGenericDefinition(length),
                _ => throw new Exception("Unreachable")
            };
        }

        public static int GetTupleTypeLength(Type tupleType)
        {
            if (!IsReferenceTupleGenericDefinition(tupleType) && !IsValueTupleGenericDefinition(tupleType))
            {
                throw new InvalidOperationException($"Type is not a valid tuple type: {tupleType}");
            }

            var t = tupleType.GetGenericTypeDefinition();

            if (tupleType.IsClass)
            {
                return t switch
                {
                    Type _ when t == typeof(Tuple<>) => 1,
                    Type _ when t == typeof(Tuple<,>) => 2,
                    Type _ when t == typeof(Tuple<,,>) => 3,
                    Type _ when t == typeof(Tuple<,,,>) => 4,
                    Type _ when t == typeof(Tuple<,,,,>) => 5,
                    Type _ when t == typeof(Tuple<,,,,,>) => 6,
                    Type _ when t == typeof(Tuple<,,,,,,>) => 6,
                    Type _ when t == typeof(Tuple<,,,,,,,>) => 8,
                    _ => throw new Exception("Unreachable")
                };
            }

            if (tupleType.IsValueType)
            {
                return t switch
                {
                    Type _ when t == typeof(ValueTuple<>) => 1,
                    Type _ when t == typeof(ValueTuple<,>) => 2,
                    Type _ when t == typeof(ValueTuple<,,>) => 3,
                    Type _ when t == typeof(ValueTuple<,,,>) => 4,
                    Type _ when t == typeof(ValueTuple<,,,,>) => 5,
                    Type _ when t == typeof(ValueTuple<,,,,,>) => 6,
                    Type _ when t == typeof(ValueTuple<,,,,,,>) => 6,
                    Type _ when t == typeof(ValueTuple<,,,,,,,>) => 8,
                    _ => throw new Exception("Unreachable")
                };
            }

            throw new Exception($"Invalid type '{tupleType}'");
        }

        public static Type MakeOneElementTupleType(Type type, TupleKind kind)
        {
            return kind switch
            {
                TupleKind.ValueType => typeof(ValueTuple<>).MakeGenericType(type),
                TupleKind.Reference => typeof(Tuple<>).MakeGenericType(type),
                _ => throw new Exception("Unreachable")
            };
        }

        public static ITuple CreateTuple(Type[] types, object?[] values, TupleKind kind)
        {
            var tupleGenericDefinition = GetTupleGenericDefinition(types.Length, kind);
            var genericTupleType = tupleGenericDefinition.MakeGenericType(types);
            return (ITuple)Activator.CreateInstance(genericTupleType, values)!;
        }

        public static ITuple CreateTuple(Type type, object? value, TupleKind kind)
        {
            var tupleGenericDefinition = GetTupleGenericDefinition(1, kind);
            var genericTupleType = tupleGenericDefinition.MakeGenericType(type);
            return (ITuple)Activator.CreateInstance(genericTupleType, value)!;
        }

        public static bool IsBuiltinTupleType(Type type)
        {
            if (type.IsValueType)
            {
                return IsValueTupleGenericDefinition(type);
            }
            else
            {
                return IsReferenceTupleGenericDefinition(type);
            }
        }

        public static bool IsValueTupleGenericDefinition(Type type)
        {
            Type t = type.GetGenericTypeDefinition();

            switch (t)
            {
                case Type _ when t == typeof(ValueTuple<>):
                case Type _ when t == typeof(ValueTuple<,>):
                case Type _ when t == typeof(ValueTuple<,,>):
                case Type _ when t == typeof(ValueTuple<,,,>):
                case Type _ when t == typeof(ValueTuple<,,,,>):
                case Type _ when t == typeof(ValueTuple<,,,,,>):
                case Type _ when t == typeof(ValueTuple<,,,,,,>):
                case Type _ when t == typeof(ValueTuple<,,,,,,,>):
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsReferenceTupleGenericDefinition(Type type)
        {
            Type t = type.GetGenericTypeDefinition();

            switch (t)
            {
                case Type _ when t == typeof(Tuple<>):
                case Type _ when t == typeof(Tuple<,>):
                case Type _ when t == typeof(Tuple<,,>):
                case Type _ when t == typeof(Tuple<,,,>):
                case Type _ when t == typeof(Tuple<,,,,>):
                case Type _ when t == typeof(Tuple<,,,,,>):
                case Type _ when t == typeof(Tuple<,,,,,,>):
                case Type _ when t == typeof(Tuple<,,,,,,,>):
                    return true;
                default:
                    return false;
            }
        }
    }
}
