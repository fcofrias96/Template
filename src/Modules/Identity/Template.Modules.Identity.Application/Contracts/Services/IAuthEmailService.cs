using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface IAuthEmailService
    {
        Task SendConfirmationEmailAsync(ApplicationUser user, string confirmationLink, CancellationToken cancellationToken);
        Task SendChangeEmailNotificationAsync(ApplicationUser user, string newEmail, string changeEmailLink, CancellationToken cancellationToken);
        Task SendForgotPasswordEmailAsync(ApplicationUser user, string resetPasswordUrl, CancellationToken cancellationToken);
    }
}
