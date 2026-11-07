namespace Template.SharedKernel.Results
{
    /// <summary>
    /// Outcome of an operation that can fail in an expected way. Use it instead of throwing for business
    /// failures (not found, invalid input, forbidden, ...); keep exceptions for bugs and infrastructure faults.
    /// An <see cref="Error"/> converts implicitly, so a method can simply <c>return SomeErrors.X;</c>.
    /// </summary>
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
            {
                throw new ArgumentException("A successful result cannot carry an error.", nameof(error));
            }

            if (!isSuccess && error == Error.None)
            {
                throw new ArgumentException("A failed result must carry an error.", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

        public static implicit operator Result(Error error) => Failure(error);
    }

    /// <summary>
    /// <see cref="Result"/> that carries a value when it succeeds.
    /// </summary>
    public sealed class Result<TValue> : Result
    {
        private readonly TValue? _value;

        internal Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        /// <summary>The value of a successful result. Accessing it on a failure is a bug and throws.</summary>
        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failed result cannot be accessed.");

        /// <summary>Runs <paramref name="next"/> on success; a failure short-circuits and is passed through.</summary>
        public async Task<Result<TOut>> BindAsync<TOut>(Func<TValue, Task<Result<TOut>>> next)
            => IsSuccess ? await next(Value) : Failure<TOut>(Error);

        /// <summary>Transforms the value on success; a failure short-circuits and is passed through.</summary>
        public async Task<Result<TOut>> MapAsync<TOut>(Func<TValue, Task<TOut>> map)
            => IsSuccess ? Success(await map(Value)) : Failure<TOut>(Error);

        public static implicit operator Result<TValue>(TValue value) => Success(value);

        public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
    }
}
