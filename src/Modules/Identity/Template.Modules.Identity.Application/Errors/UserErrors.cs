using Microsoft.AspNetCore.Identity;
using Template.BuildingBlocks.Application.Errors;
using Template.SharedKernel.Results;

namespace Template.Modules.Identity.Application.Errors
{
    public static class UserErrors
    {
        public static Error NotFound(int userId) => EntityErrors.NotFound("User", userId);

        public static readonly Error IdMismatch = Error.Validation(
            "User.IdMismatch",
            "The provided user ID does not match the ID in the request data.");

        public static readonly Error Forbidden = Error.Forbidden(
            "User.Forbidden",
            "You are not authorized to access this resource.");

        public static Error UpdateFailed(IdentityResult result) => Error.Validation(
            "User.UpdateFailed",
            $"User update failed: {result.Describe()}");
    }
}
