using Template.SharedKernel.Results;

namespace Template.BuildingBlocks.Application.Errors
{
    /// <summary>Errors shared by every entity handled through the base services.</summary>
    public static class EntityErrors
    {
        public static readonly Error InvalidId = Error.Validation(
            "Request.InvalidId",
            "The request id must be greater than zero.");

        public static readonly Error IdMismatch = Error.Validation(
            "Request.IdMismatch",
            "The route id does not match the payload id.");

        public static Error NotFound(string entityName, object? id) => Error.NotFound(
            $"{entityName}.NotFound",
            $"{entityName} with id ({id}) was not found");
    }
}
