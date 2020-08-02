using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastCSV.Utils
{
    enum ValuePosition
    {
        Left, Right
    }

    /// <summary>
    /// Represents a value of two possible types.
    /// </summary>
    /// <typeparam name="L"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <seealso cref="System.IEquatable{FastCSV.Utils.Either{L, R}}" />
    [Serializable]
    public readonly struct Either<L, R> : IEquatable<Either<L, R>>
    {
        internal readonly object _value;
        private readonly ValuePosition _position;

        internal Either(object value, ValuePosition position)
        {
            _value = value;
            _position = position;
        }

        public L Left
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_position != ValuePosition.Left || _value == null)
                {
                    throw new InvalidOperationException("Value is not in the left");
                }

                return (L)_value;
            }
        }

        public R Right
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_position != ValuePosition.Right || _value == null)
                {
                    throw new InvalidOperationException("Value is not in the right");
                }

                return (R)_value;
            }
        }

        public bool IsRight => _position == ValuePosition.Right;

        public bool IsLeft => _position == ValuePosition.Left;

        public Either<R, L> Swap()
        {
            return _position switch
            {
                ValuePosition.Left => Either.FromRight<R, L>(Left),
                ValuePosition.Right => Either.FromLeft<R, L>(Right),
                _ => throw new InvalidOperationException()
            };
        }

        public Either<L, TResult> MapRight<TResult>(Func<R, TResult> mapper)
        {
            if (IsRight)
            {
                return Either.FromRight<L, TResult>(mapper(Right));
            }
            else
            {
                return Either.FromLeft<L, TResult>(Left);
            }
        }

        public Either<TResult, R> MapLeft<TResult>(Func<L, TResult> mapper)
        {
            if (IsLeft)
            {
                return Either.FromLeft<TResult, R>(mapper(Left));
            }
            else
            {
                return Either.FromRight<TResult, R>(Right);
            }
        }

        public TResult Match<TResult>(Func<R, TResult> ifRight, Func<L, TResult> ifLeft)
        {
            if (IsRight)
            {
                return ifRight(Right);
            }
            else
            {
                return ifLeft(Left);
            }
        }

        public override string ToString()
        {
            return _position switch
            {
                ValuePosition.Left => $"Left({_value})",
                ValuePosition.Right => $"Right({_value})",
                _ => throw new InvalidOperationException(),
            };
        }

        public override bool Equals(object? obj)
        {
            return obj is Either<L, R> either && Equals(either);
        }

        public bool Equals(Either<L, R> other)
        {
            return EqualityComparer<object>.Default.Equals(_value, other._value) &&
                   _position == other._position;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value, _position);
        }

        public static bool operator ==(Either<L, R> left, Either<L, R> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Either<L, R> left, Either<L, R> right)
        {
            return !(left == right);
        }
    }

    public static class Either
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, R> FromLeft<L, R>(L left)
        {
            return new Either<L, R>(left!, ValuePosition.Left);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Either<L, R> FromRight<L, R>(R right)
        {
            return new Either<L, R>(right!, ValuePosition.Right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Reduce<T>(this Either<T, T> either)
        {
            return either.IsRight ? either.Right : either.Left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetUnderlyingType<T ,R>(this Either<T, R> either)
        {
            return either._value.GetType();
        }
    }
}
