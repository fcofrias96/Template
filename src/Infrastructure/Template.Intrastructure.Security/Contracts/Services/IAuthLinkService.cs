using Template.Domain.Entities.V1;
using Template.Security.Entities;

public interface IAuthLinkService
{
    Task<string> GenerateEmailConfirmationLinkAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<string> GenerateChangeEmailLinkAsync(ApplicationUser user, string newEmail, CancellationToken cancellationToken);
    Task<string> GenerateResetPasswordLinkAsync(ApplicationUser user, string encodedToken);
}
