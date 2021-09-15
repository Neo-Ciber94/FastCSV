using System;
using System.Collections.Generic;

namespace FastCSV.Utils
{
    /// <summary>
    /// Represents with one of two states.
    /// </summary>
    /// <typeparam name="TLeft">The type of the value on the left.</typeparam>
    /// <typeparam name="TRight">The type of the value on the right.</typeparam>
    public readonly struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
    {
        private readonly object _value;
        private readonly bool _isRight;

        internal Either(object value, bool isRight)
        {
            _value = value;
            _isRight = isRight;
        }

        /// <summary>
        /// The value is stored in the right.
        /// </summary>
        public bool IsRight => _isRight;

        /// <summary>
        /// The value is stored in the left.
        /// </summary>
        public bool IsLeft => !IsRight;

        /// <summary>
        /// Gets the value in the left.
        /// </summary>
        public TLeft Left => IsLeft ? (TLeft)_value : throw new InvalidOperationException("value is right");

        /// <summary>
        /// Gets the value in the right.
        /// </summary>
        public TRight Right => IsRight ? (TRight)_value : throw new InvalidOperationException("value is left");

        /// <summary>
        /// Maps either the right and left value of this instance.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="left">Function to map the left value.</param>
        /// <param name="right">Function to map the right value.</param>
        /// <returns>Resulting value.</returns>
        public TResult Fold<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right)
        {
            if (IsRight)
            {
                return right(Right);
            }
            else
            {
                return left(Left);
            }
        }

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

        public override string ToString()
        {
            return IsLeft ? $"Left({Left})" : $"Right({Right})";
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
        /// <summary>
        /// Creates an <see cref="Either{TLeft, TRight}"/> from the left value.
        /// </summary>
        /// <typeparam name="TLeft">Type of the left value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>An either with a left value.</returns>
        public static Either<TLeft, Nothing> FromLeft<TLeft>(TLeft value)
        {
            return FromLeft<TLeft, Nothing>(value);
        }

        /// <summary>
        /// Creates an <see cref="Either{TLeft, TRight}"/> from the left value.
        /// </summary>
        /// <typeparam name="TLeft">Type of the left value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>An either with a left value.</returns>
        public static Either<TLeft, TRight> FromLeft<TLeft, TRight>(TLeft value)
        {
            return new Either<TLeft, TRight>(value!, isRight: false);
        }

        /// <summary>
        /// Creates an <see cref="Either{TLeft, TRight}"/> from the right value.
        /// </summary>
        /// <typeparam name="TRight">Type of the right value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>An either with a right value.</returns>
        public static Either<Nothing, TRight> FromRight<TRight>(TRight value)
        {
            return FromRight<Nothing, TRight>(value);
        }

        /// <summary>
        /// Creates an <see cref="Either{TLeft, TRight}"/> from the right value.
        /// </summary>
        /// <typeparam name="TRight">Type of the right value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>An either with a right value.</returns>
        public static Either<TLeft, TRight> FromRight<TLeft, TRight>(TRight value)
        {
            return new Either<TLeft, TRight>(value!, isRight: true);
        }
    }
}
