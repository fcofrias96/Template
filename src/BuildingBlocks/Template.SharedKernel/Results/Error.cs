namespace Template.SharedKernel.Results
{
    /// <summary>
    /// An expected failure: a stable machine-readable <see cref="Code"/> (e.g. "User.NotFound"),
    /// a human-readable <see cref="Description"/> and its <see cref="Type"/>.
    /// </summary>
    public sealed record Error(string Code, string Description, ErrorType Type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

        public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);

        public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);

        public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);

        public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);

        public static Error Unauthorized(string code, string description) => new(code, description, ErrorType.Unauthorized);

        public static Error Forbidden(string code, string description) => new(code, description, ErrorType.Forbidden);
    }
}
