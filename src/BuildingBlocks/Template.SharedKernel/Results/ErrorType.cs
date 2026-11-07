namespace Template.SharedKernel.Results
{
    /// <summary>
    /// Category of an expected failure. The web layer maps each one to an HTTP status code.
    /// </summary>
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbidden = 5
    }
}
