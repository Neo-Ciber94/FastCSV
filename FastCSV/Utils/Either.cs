using System;
using System.Collections.Generic;

namespace FastCSV.Utils
{
    public readonly struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
    {
        private readonly object _value;
        private readonly bool _isRight;

        internal Either(object value, bool isRight)
        {
            _value = value;
            _isRight = isRight;
        }

        public bool IsRight => _isRight;

        public bool IsLeft => !IsRight;

        public TLeft Left => IsLeft ? (TLeft)_value : throw new InvalidOperationException("value is right");

        public TRight Right => IsRight ? (TRight)_value : throw new InvalidOperationException("value is left");

        public override bool Equals(object? obj)
        {
            return obj is Either<TLeft, TRight> either && Equals(either);
        }

        public bool Equals(Either<TLeft, TRight> other)
        {
            return EqualityComparer<object>.Default.Equals(_value, other._value) &&
                   _isRight == other._isRight;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value, _isRight);
        }

        public static bool operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        {
            return !(left == right);
        }

        public static implicit operator Either<TLeft, TRight>(Either<TLeft, Nothing> value)
        {
            return new Either<TLeft, TRight>(value._value, isRight: false);
        }

        public static implicit operator Either<TLeft, TRight>(Either<Nothing, TRight> value)
        {
            return new Either<TLeft, TRight>(value._value, isRight: true);
        }
    }

    public static partial class Either
    {
        public static Either<TLeft, Nothing> FromLeft<TLeft>(TLeft value)
        {
            return FromLeft<TLeft, Nothing>(value);
        }

        public static Either<TLeft, TRight> FromLeft<TLeft, TRight>(TLeft value)
        {
            return new Either<TLeft, TRight>(value!, isRight: false);
        }

        public static Either<Nothing, TRight> FromRight<TRight>(TRight value)
        {
            return FromRight<Nothing, TRight>(value);
        }

        public static Either<TLeft, TRight> FromRight<TLeft, TRight>(TRight value)
        {
            return new Either<TLeft, TRight>(value!, isRight: true);
        }
    }
}
