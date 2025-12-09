using Template.Security.Entities;

namespace Template.Security.Contracts.Services
{
    public interface IAuthEmailService
    {
        Task SendConfirmationEmailAsync(User user, string confirmationLink, CancellationToken cancellationToken);
        Task SendChangeEmailNotificationAsync(User user, string newEmail, string changeEmailLink, CancellationToken cancellationToken);
        Task SendForgotPasswordEmailAsync(User user, string resetPasswordUrl, CancellationToken cancellationToken);
    }
}