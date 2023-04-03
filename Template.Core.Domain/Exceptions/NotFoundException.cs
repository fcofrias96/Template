namespace Template.Core.Domain.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object? key) : base($"{name} with id ({key}) was not found")
        {
        }

        public NotFoundException(string name) : base($"{name} was not found")
        {
        }
    }
}