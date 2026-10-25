using Template.Modules.Notifications.Contracts;

namespace Template.Modules.Notifications.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto emailDto, CancellationToken cancellationToken);
    }

}
