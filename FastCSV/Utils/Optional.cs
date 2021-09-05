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
    public readonly struct Optional<T> : IEquatable<Optional<T>> where T: notnull
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

        /// <summary>
        /// Maps the values from this optional.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <typeparam name="TResult">Type of the resulting optional.</typeparam>
        /// <param name="ifSome"></param>
        /// <param name="ifNone"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<TResult> Match<TResult>(Func<T, TResult> ifSome, Action ifNone) where TResult : notnull
        {
            if (HasValue)
            {
                TResult result = ifSome(Value);
                return new Optional<TResult>(result);
            }
            else
            {
                ifNone();
                return default;
            }
        }

        /// <summary>
        /// Maps the value of this optional if have a value.
        /// </summary>
        /// <typeparam name="TResult">The resulting type of the value.</typeparam>
        /// <param name="mapper">The function to map the value.</param>
        /// <returns>An optional with the mapped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : notnull
        {
            if (HasValue)
            {
                TResult result = mapper(Value);
                return new Optional<TResult>(result);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Filters the value of this optional if have a value.
        /// </summary>
        /// <param name="predicate">The predicate to filter the value.</param>
        /// <returns>The optional with the filtered value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Optional<T> Filter(Predicate<T> predicate)
        {
            if (HasValue)
            {
                T value = Value;
                if (predicate(value))
                {
                    return this;
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

        public static implicit operator Optional<T>(Optional<Nothing> _)
        {
            return new Optional<T>();
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
        /// <summary>
        /// Creates an <see cref="Optional{T}"/> from the given value.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value of the optional.</param>
        /// <returns>An optional with the given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Some<T>(T value) where T: notnull
        {
            return new Optional<T>(value);
        }

        /// <summary>
        /// Creates an <see cref="Optional{T}"/> with no value.
        /// </summary>
        /// <returns>An optional with no value.</returns>
        public static Optional<Nothing> None()
        {
            /// Optional<Nothing> implicitly converts to an empty optional
            return default;
        }

        /// <summary>
        /// Creates an <see cref="Optional{T}"/> with no value.
        /// </summary>
        /// <typeparam name="T">Type of the optional</typeparam>
        /// <returns>An optional with no value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> None<T>() where T : notnull
        {
            return default;
        }

        /// <summary>
        /// Converts a nested optional into a single one.
        /// </summary>
        /// <typeparam name="T">Type of the optional value.</typeparam>
        /// <param name="optional">The optional to flatten.</param>
        /// <returns>An optional that may contains a value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Optional<T> Flatten<T>(this in Optional<Optional<T>> optional) where T : notnull
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
    }
}
