using System;
using System.Runtime.CompilerServices;

namespace FastCSV.Benchmarks
{
    public readonly ref struct Result<T, E>
    {
        private readonly RawReference _value;
        private readonly bool _isOk;

        public Result(T value)
        {
            _value = RawReference.From(ref value);
            _isOk = true;
        }

        public Result(E error)
        {
            _value = RawReference.From(ref error);
            _isOk = false;
        }

        public bool IsOk => _isOk;
        public bool IsError => !_isOk;

        public T Value
        {
            get
            {
                if (IsError)
                {
                    throw new InvalidOperationException("The result is an error");
                }

                return _value.As<T>();
            }
        }

        public E Error
        {
            get
            {
                if (IsOk)
                {
                    throw new InvalidOperationException("The result is ok");
                }

                return _value.As<E>();
            }
        }

        public static implicit operator Result<T, E>(ResultOk<T> result)
        {
            return new Result<T, E>(result._value);
        }

        public static implicit operator Result<T, E>(ResultError<E> result)
        {
            return new Result<T, E>(result._error);
        }

        public override string ToString()
        {
            return _isOk ? $"Ok({Value})" : $"Error({Error})";
        }
    }

    public readonly ref struct ResultOk<T>
    {
        internal readonly T _value;

        internal ResultOk(T value)
        {
            _value = value;
        }
    }

    public readonly ref struct ResultError<E>
    {
        internal readonly E _error;

        internal ResultError(E error)
        {
            _error = error;
        }
    }

    public readonly struct _Result<T, E>
    {

    }

    public static class Result
    {
        public static ResultOk<T> Ok<T>(T value) => new ResultOk<T>(value);

        public static ResultError<E> Error<E>(E error) => new ResultError<E>(error);
    }

    public unsafe ref struct RawReference
    {
        private readonly void* _value;

        private RawReference(void* ptr)
        {
            _value = ptr;
        }

        public static RawReference From<T>(ref T value)
        {
            return new RawReference(Unsafe.AsPointer<T>(ref value));
        }

        public ref T As<T>() => ref Unsafe.AsRef<T>(_value);
    }
}