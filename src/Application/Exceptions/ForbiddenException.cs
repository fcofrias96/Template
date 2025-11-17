namespace Template.Application.Exceptions
{
    public class ForbiddenException : Exception
    {
        private const string DefaultMessage = "You are not authorized to access this resource.";

        public ForbiddenException() : base(DefaultMessage)
        {
        }

        public ForbiddenException(string message) : base(message)
        {
        }

        public ForbiddenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}