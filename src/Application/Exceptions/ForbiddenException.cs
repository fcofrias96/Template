namespace Template.Application.Exceptions
{
    public class ForbiddenException : ApplicationException
    {
        private const string message = "You are not authorized to access this resource.";

        public ForbiddenException() : base(message)
        {
        }

        public ForbiddenException(string message) : base(message)
        {
        }
    }
}