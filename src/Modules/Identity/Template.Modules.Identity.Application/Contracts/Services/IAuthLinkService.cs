using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Contracts.Services;

public interface IAuthLinkService
{
    Task<string> GenerateEmailConfirmationLinkAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<string> GenerateChangeEmailLinkAsync(ApplicationUser user, string newEmail, CancellationToken cancellationToken);
    Task<string> GenerateResetPasswordLinkAsync(ApplicationUser user, string encodedToken);
}
