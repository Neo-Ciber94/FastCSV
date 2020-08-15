using System;
using System.Runtime.CompilerServices;

namespace FastCSV.Benchmarks
{
    public readonly struct Result<T, TError>
    {
        private readonly T _value;
        private readonly TError _error;
        private readonly bool _hasValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result(T value)
        {
            _value = value;
            _error = default;
            _hasValue = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Result(TError error)
        {
            _value = default;
            _error = error;
            _hasValue = false;
        }

        public bool HasValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _hasValue;
        }

        public bool HasError
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !_hasValue;
        }

        public T Value
        {
            get
            {
                if (!_hasValue)
                {
                    throw new InvalidOperationException("Result has an error");
                }

                return _value;
            }
        }

        public TError Error
        {
            get
            {
                if (_hasValue)
                {
                    throw new InvalidOperationException("Result has a value");
                }

                return _error;
            }
        }

        public override string ToString()
        {
            return _hasValue ? $"Ok({_value})" : $"Error({_error})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<T, TError>(ResultOk<T> result) => new Result<T, TError>(result.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<T, TError>(ResultError<TError> result) => new ResultError<TError>(result.Error);
    }

    public static class Result
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResultOk<T> Ok<T>(T value) => new ResultOk<T>(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResultError<TError> Error<TError>(TError error) => new ResultError<TError>(error);
    }

    public readonly struct ResultOk<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ResultOk(T value)
        {
            Value = value;
        }

        public T Value 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get; 
        }
    }

    public readonly struct ResultError<TError>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ResultError(TError error)
        {
            Error = error;
        }

        public TError Error 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get; 
        }
    }
}