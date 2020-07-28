using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FastCSV.Utils
{
    /// <summary>
    /// Represents an optional value, that can be present or not.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    /// <seealso cref="IEquatable{Optional{T}}" />
    [Serializable]
    public readonly struct Optional<T> : IEquatable<Optional<T>>
    {
        internal readonly T _value;
        internal readonly bool _hasValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Optional{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">If the value is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional(T value)
        {
            _value = value ?? throw new ArgumentNullException("Optional<T> cannot contain a null value");
            _hasValue = true;
        }

        /// <summary>
        /// Gets a value indicating whether this optional has value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this optional has value; otherwise, <c>false</c>.
        /// </value>
        public bool HasValue 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _hasValue;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <exception cref="InvalidOperationException">If the optional has no value.</exception>
        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _hasValue ? _value : throw new InvalidOperationException("Optional is empty");
        }

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValueOrDefault(T defaultValue)
        {
            return HasValue ? _value : defaultValue;
        }

        /// <summary>
        /// Determines whether this optional contains the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T value)
        {
            return HasValue && EqualityComparer<T>.Default.Equals(_value, value);
        }

        public override string ToString()
        {
            return HasValue ? $"Optional({_value})" : "None";
        }

        public override bool Equals(object? obj)
        {
            return obj is Optional<T> optional && Equals(optional);
        }

        public bool Equals(Optional<T> other)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value) &&
                   _hasValue == other._hasValue;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value, _hasValue);
        }

        public static bool operator ==(Optional<T> left, Optional<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Optional<T> left, Optional<T> right)
        {
            return !(left == right);
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static implicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }
    }

    public static class Optional
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Some<T>(T value)
        {
            return new Optional<T>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> None<T>()
        {
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Flatten<T>(this in Optional<Optional<T>> optional)
        {
            if (optional.HasValue)
            {
                return optional.Value;
            }
            else
            {
                return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<TResult> Match<T, TResult>(this in Optional<T> optional, Func<T, TResult> ifSome, Action ifNone)
        {
            if (optional.HasValue)
            {
                TResult result = ifSome(optional.Value);
                return new Optional<TResult>(result);
            }
            else
            {
                ifNone();
                return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<TResult> Map<T, TResult>(this in Optional<T> optional, Func<T, TResult> mapper)
        {
            if (optional.HasValue)
            {
                TResult result = mapper(optional.Value);
                return new Optional<TResult>(result);
            }
            else
            {
                return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Filter<T>(this in Optional<T> optional, Predicate<T> predicate)
        {
            if (optional.HasValue)
            {
                T value = optional.Value;
                if (predicate(value))
                {
                    return optional;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }

        public static unsafe T Take<T>(this in Optional<T> optional)
        {
            throw null!;
        }

        public static unsafe void Replace<T>(this in Optional<T> optional, T newValue)
        {
            throw null!;
        }
    }
}
