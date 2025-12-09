using Template.Security.Entities;

public interface IAuthLinkService
{
    Task<string> GenerateEmailConfirmationLinkAsync(User user, CancellationToken cancellationToken);
    Task<string> GenerateChangeEmailLinkAsync(User user, string newEmail, CancellationToken cancellationToken);
    Task<string> GenerateResetPasswordLinkAsync(User user, string encodedToken);
}
