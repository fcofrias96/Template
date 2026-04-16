using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services
{
    public interface IAuthEmailService
    {
        Task SendConfirmationEmailAsync(ApplicationUser user, string confirmationLink, CancellationToken cancellationToken);
        Task SendChangeEmailNotificationAsync(ApplicationUser user, string newEmail, string changeEmailLink, CancellationToken cancellationToken);
        Task SendForgotPasswordEmailAsync(ApplicationUser user, string resetPasswordUrl, CancellationToken cancellationToken);
    }
}