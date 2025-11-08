namespace Template.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested entity is not found.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with the specified entity name and key.
        /// </summary>
        /// <param name="name">The name of the entity that was not found.</param>
        /// <param name="key">The key/identifier of the entity that was not found.</param>
        public NotFoundException(string name, object? key) 
            : base($"{name} with id ({key}) was not found")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with the specified entity name.
        /// </summary>
        /// <param name="name">The name of the entity that was not found.</param>
        public NotFoundException(string name) 
            : base($"{name} was not found")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a custom message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}